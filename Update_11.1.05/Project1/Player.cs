using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Project1
{
    public class Player
    {
        GameManager gm;
        Texture2D tex;
        SpriteFont font;

        int frameX = 0;
        int frameY = 1; // facing down by default
        float animTimer = 0f;
        float animSpeed = 0.10f;
        int maxFrames = 5;
        int lastRow = 1;

        public Vector2 Position;
        public float Speed = 360f;
        public float SprintMultiplier = 1.7f;

        //effect
        public float speedDebuffTimer = 0f;
        public float originalSpeed = 360f;
        public float debuffedSpeed = 260f;

        public int MaxHealth = 100;
        public int Health = 100;

        public int MaxStamina = 100;
        public float Stamina = 100f;
        public float StaminaDrainRate = 25f;
        public float StaminaRegenRate = 15f;
        public bool staminaLocked = false;
        public int StaminaSprintThreshold = 80;

        public bool IsAiming = false;
        public bool IsDead => Health <= 0;

        public bool WasHit = false;
        float hitTimer = 0f;

        public bool HasGun = false;
        public Inventory Inventory => gm.Inventory;

        public float FlashlightAlpha = 0f;
        public bool FlashlightOn = false;
        public bool WeaponLightOn = false;
        public float FlashlightTargetAlpha = 0f;
        public float FlashlightInterpSpeed = 4f;
        public float Battery = 100f;
        public float BatteryDrainRate = 5f;
        public float WeaponLightExtraDrain = 8f;
        public float BatteryRegenRate = 6f;
        public int BatteryMinToEnable = 30;

        public int AmmoInMagazine = 12;
        public int MagazineSize = 12;
        public int AmmoReserve => gm.Inventory.AmmoReserve;

        // Weapon
        public int Ammo = 12;
        public float ShootCooldown = 0.2f;
        float shootTimer = 0f;

        // Reload
        float reloadTimer = 0f;
        bool isReloading = false;
        float reloadDuration = 2f;

        public bool IsPaused = false;
        public bool IsStuck = false;
        public bool IsTalking = false;

        // Sound
        SoundEffect walk_dirt => gm.WalkDirtSfx;
        SoundEffect walk_wood => gm.WalkWoodSfx;
        SoundEffectInstance walkInstance;

        SoundEffect hit => gm.Hit;
        SoundEffectInstance hitInstance;

        SoundEffect shoot => gm.Shoot;
        SoundEffectInstance shootInstance;

        SoundEffect pickup => gm.PickUp;
        SoundEffectInstance pickupInstance;

        float walkDirtVolume = 0.4f;
        float walkWoodVolume = 0.4f;

        float walkDirtInterval = 0.38f;
        float walkWoodInterval = 0.36f;

        float walkTimer = 0f;

        public Player(GameManager gm, Texture2D tex, SpriteFont font)
        {
            this.gm = gm;
            this.tex = tex;
            this.font = font;
            Position = Vector2.Zero;
        }

        public void Update(GameTime gt)
        {
            if (IsPaused) return;

            float dt = (float)gt.ElapsedGameTime.TotalSeconds;

            float currentSpeed = Speed;
            if (speedDebuffTimer > 0)
            {
                currentSpeed = debuffedSpeed;
                speedDebuffTimer -= dt;
            }

            if (IsTalking) return;

            if (!IsStuck)
            {
                // Movement
                var ks = InputManager.KeyState;
                Vector2 dir = Vector2.Zero;
                if (ks.IsKeyDown(Keys.W)) dir.Y -= 1;
                if (ks.IsKeyDown(Keys.S)) dir.Y += 1;
                if (ks.IsKeyDown(Keys.A)) dir.X -= 1;
                if (ks.IsKeyDown(Keys.D)) dir.X += 1;
                if (dir.LengthSquared() > 0) dir.Normalize();

                bool wantsToSprint = ks.IsKeyDown(Keys.LeftShift);
                bool canSprint = wantsToSprint && !staminaLocked && Stamina > 0 && dir != Vector2.Zero;
                float speedToUse = currentSpeed * (canSprint ? SprintMultiplier : 0.8f);
                Position += dir * speedToUse * dt;

                // Stamina
                if (canSprint)
                {
                    Stamina -= StaminaDrainRate * dt;
                    if (Stamina <= 0) { Stamina = 0; staminaLocked = true; }
                }
                else
                {
                    Stamina += StaminaRegenRate * dt;
                    if (Stamina > MaxStamina) Stamina = MaxStamina;
                    if (staminaLocked && Stamina >= StaminaSprintThreshold) staminaLocked = false;
                }

                // Animation
                Vector2 mouseWorld = InputManager.MouseState.Position.ToVector2() + CameraOffset(gt);
                if (IsAiming)
                {
                    Vector2 mouseDir = mouseWorld - Position;
                    float angle = MathF.Atan2(mouseDir.Y, mouseDir.X);
                    if (angle >= -MathF.PI / 4 && angle <= MathF.PI / 4) frameY = 2;
                    else if (angle > MathF.PI / 4 && angle < 3 * MathF.PI / 4) frameY = 1;
                    else if (angle < -MathF.PI / 4 && angle > -3 * MathF.PI / 4) frameY = 4;
                    else frameY = 3;
                }

                if (dir != Vector2.Zero)
                {
                    animSpeed = canSprint ? 0.06f : 0.10f;
                    if (dir.Y > 0) frameY = 0;
                    if (dir.X > 0) frameY = 3;
                    if (dir.X < 0) frameY = 2;
                    if (dir.Y < 0) frameY = 1;
                    lastRow = frameY;

                    animTimer += dt;
                    if (animTimer >= animSpeed)
                    {
                        frameX = (frameX + 1) % maxFrames;
                        animTimer = 0f;
                    }
                }
                else
                {
                    frameX = 0;
                    frameY = lastRow;
                }

                // Walk sound
                walkTimer -= dt;
                if (dir != Vector2.Zero)
                {
                    bool inLighthouse = gm.InLighthouse;
                    float interval = inLighthouse ? walkWoodInterval : walkDirtInterval;
                    float vol = inLighthouse ? walkWoodVolume : walkDirtVolume;
                    var sfx = inLighthouse ? walk_wood : walk_dirt;

                    if (walkTimer <= 0)
                    {
                        walkInstance?.Stop();
                        walkInstance = sfx.CreateInstance();
                        walkInstance.Volume = vol;
                        walkInstance.Play();
                        walkTimer = (canSprint ? interval * 0.67f : interval);
                    }
                }
                else
                {
                    walkTimer = 0f;
                    walkInstance?.Stop();
                    walkInstance = null;
                }

                // Flashlight
                if (InputManager.KeyPressed(Keys.Space))
                {
                    if (!FlashlightOn && Battery >= BatteryMinToEnable)
                    {
                        FlashlightOn = true;
                        FlashlightTargetAlpha = 1f;
                    }
                    else
                    {
                        FlashlightOn = false;
                        FlashlightTargetAlpha = 0f;
                    }
                }
                FlashlightAlpha = MathHelper.Lerp(FlashlightAlpha, FlashlightTargetAlpha, MathHelper.Clamp(FlashlightInterpSpeed * dt, 0f, 1f));

                bool lightActive = FlashlightTargetAlpha > 0 || WeaponLightOn;
                float drain = 0f;
                if (FlashlightTargetAlpha > 0) drain += BatteryDrainRate;
                if (WeaponLightOn) drain += WeaponLightExtraDrain;
                if (FlashlightTargetAlpha > 0 && WeaponLightOn) drain *= 2f;

                if (lightActive)
                {
                    Battery -= drain * dt;
                    if (Battery <= 0f)
                    {
                        Battery = 0f;
                        FlashlightOn = false;
                        WeaponLightOn = false;
                        FlashlightTargetAlpha = 0f;
                    }
                }
                else
                {
                    Battery += BatteryRegenRate * dt;
                    if (Battery > 100f) Battery = 100f;
                }

                // Aiming
                IsAiming = InputManager.MouseRightDown();

                var currentSlot = Inventory.Hotbar[Inventory.SelectedIndex];
                bool holdingGun = currentSlot != null && currentSlot.Item != null && currentSlot.Item.Id == "gun";

                // Shooting
                shootTimer -= dt;
                if (!isReloading && holdingGun && IsAiming && InputManager.MouseLeftPressed() && shootTimer <= 0 && AmmoInMagazine > 0 && gm.InMainLevel)
                {
                    Shoot(mouseWorld);
                    shootTimer = ShootCooldown;
                }

                // Reload with 2s
                if (!isReloading && holdingGun && InputManager.KeyPressed(Keys.R))
                {
                    int need = MagazineSize - AmmoInMagazine;
                    if (need > 0 && gm.Inventory.AmmoReserve > 0)
                    {
                        isReloading = true;
                        reloadTimer = reloadDuration;
                    }
                }

                if (isReloading)
                {
                    reloadTimer -= dt;
                    if (reloadTimer <= 0f)
                    {
                        int need = MagazineSize - AmmoInMagazine;
                        int take = Math.Min(need, gm.Inventory.AmmoReserve);
                        AmmoInMagazine += take;
                        gm.Inventory.AmmoReserve -= take;
                        isReloading = false;
                    }
                }

                // Interact/pickup
                if (InputManager.KeyPressed(Keys.E))
                {
                    bool picked = gm.Level.TryPickupAt(Position);
                    if (picked)
                    {
                        pickupInstance?.Stop();
                        pickupInstance = pickup.CreateInstance();
                        pickupInstance.Volume = 0.7f;
                        pickupInstance.Play();
                    }
                }

                // Hotbar select
                for (int i = 0; i < Inventory.Hotbar.Length; i++)
                {
                    if (InputManager.KeyPressed(Keys.D1 + i))
                    {
                        Inventory.SelectedIndex = i;
                        var slot = Inventory.Hotbar[i];
                        if (slot != null && slot.Item != null)
                            Inventory.UseSelectedItem();
                    }
                }

                // Open inventory
                if (InputManager.KeyPressed(Keys.Tab)) Inventory.ToggleOpen();

                // Trap
                var trap = gm.Level.CheckTrapCollision(FootHitbox.Center.ToVector2());
                if (trap != null) trap.TriggerOnPlayer(this);

                if (WasHit)
                {
                    hitTimer -= dt;
                    if (hitTimer <= 0) WasHit = false;
                }
            }
        }

        void Shoot(Vector2 target)
        {
            AmmoInMagazine--;
            Vector2 dir = target - Position;
            if (dir.LengthSquared() == 0) dir = new Vector2(1, 0);
            dir.Normalize();
            var bullet = new Projectile(gm, Position + dir * 16, dir * 800f, gm.TexBullet, 8, 8, 10);
            gm.Level.AddProjectile(bullet);

            shootInstance?.Stop();
            shootInstance = shoot.CreateInstance();
            shootInstance.Volume = 0.7f;
            shootInstance.Play();
        }

        public Rectangle FootHitbox
        {
            get
            {
                int frameWidth = 25;
                int frameHeight = 48;
                int footHeight = 1;
                int left = (int)(Position.X - frameWidth / 2);
                int top = (int)(Position.Y - frameHeight / 2);
                return new Rectangle(left, top + frameHeight - footHeight, frameWidth, footHeight);
            }
        }

        public void TakeDamage(int dmg)
        {
            if (IsDead) return;
            Health -= dmg;
            if (Health <= 0) Health = 0;
            WasHit = true;
            hitTimer = 0.2f;
        }

        public void Draw(SpriteBatch sb, GameTime gt)
        {
            int frameWidth = 24;
            int frameHeight = 48;

            Rectangle srcRect = new Rectangle(frameX * frameWidth, frameY * frameHeight, frameWidth, frameHeight);
            Color color = Color.White;

            if (!IsTalking)
            {
                if (WasHit) color = Color.Red;
                if (IsStuck) color = Color.OrangeRed;
            }

            sb.Draw(tex, Position, srcRect, color, 0f, new Vector2(frameWidth / 2f, frameHeight / 2f), 2f, SpriteEffects.None, 0f);

            // Draw aiming laser
            var currentSlot = Inventory.Hotbar[Inventory.SelectedIndex];
            bool holdingGun = currentSlot != null && currentSlot.Item != null && currentSlot.Item.Id == "gun";

            if (gm.InMainLevel && holdingGun && IsAiming && AmmoInMagazine > 0)
            {
                Vector2 mouseWorld = InputManager.MouseState.Position.ToVector2() + CameraOffset(gt);
                Vector2 from = Position;
                Vector2 to = mouseWorld;
                Vector2 dir = to - from;
                float len = dir.Length();
                if (len > 0.1f)
                {
                    dir.Normalize();
                    float rot = (float)Math.Atan2(dir.Y, dir.X);
                    sb.Draw(gm.WhitePixel, from, null, Color.Red * 0.8f, rot, new Vector2(0, 0.5f), new Vector2(len, 1f), SpriteEffects.None, 0f);
                }
            }

            // Ammo HUD
            if (holdingGun)
            {
                string ammoTxt = $"{AmmoInMagazine}/{gm.Inventory.AmmoReserve}";
                Vector2 txtPos = Position + new Vector2(20, -40);
                sb.DrawString(gm.DefaultFont, ammoTxt, txtPos, Color.White);
            }
        }

        public void ResetAll()
        {
            Health = MaxHealth;
            Stamina = MaxStamina;
            Ammo = MagazineSize;
            Battery = 100f;
            Inventory.Clear();
            HasGun = false;
            gm.InBasement = true;
            IsStuck = false;
            IsAiming = false;
            FlashlightOn = false;
            FlashlightTargetAlpha = 0f;
            WeaponLightOn = false;
            isReloading = false;
            reloadTimer = 0f;
        }

        Vector2 CameraOffset(GameTime gt)
        {
            Matrix cam = Camera.GetTransform(gm.root, gt);
            return new Vector2(-cam.Translation.X, -cam.Translation.Y);
        }

        public void TriggerHitSound()
        {
            if (!WasHit)
            {
                WasHit = true;
                hitTimer = 0.2f;
                hitInstance?.Stop();
                hitInstance = hit.CreateInstance();
                hitInstance.Volume = 0.7f;
                hitInstance.Play();
            }
        }

        public void TriggerTrapHitSound()
        {
            TriggerHitSound();
        }
    }
}
