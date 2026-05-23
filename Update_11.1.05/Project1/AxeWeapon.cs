using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using static Project1.ProceduralLevel;

namespace Project1
{
    public class AxeWeapon
    {
        GameManager gm;
        public bool IsAttacking = false;
        public float AttackCooldown = 0.3f;
        float attackTimer = 0f;
        public int Damage = 10;

        Fence nearbyFence;

        Texture2D tex;
        Vector2 origin;

        float interactionProgress = 0f;
        float interactionTime = 2f; // ต้องกด E ต่อเนื่องกี่วินาที

        public AxeWeapon(GameManager gm)
        {
            this.gm = gm;
            tex = gm.content.Load<Texture2D>("Sprite_axe"); // sprite ขวาน
            origin = new Vector2(0, tex.Height / 2f); // หมุนจากด้ามขวาน
        }

        public void Update(GameTime gt)
        {
            float dt = (float)gt.ElapsedGameTime.TotalSeconds;
            attackTimer -= dt;

            var currentSlot = gm.Inventory.Hotbar[gm.Inventory.SelectedIndex];
            bool holdingAxe = currentSlot != null && currentSlot.Item != null && currentSlot.Item.Id == "axe";

            if (!holdingAxe) return;


            // ตรวจรั้วใกล้
            nearbyFence = gm.Level.GetNearbyFence(gm.Player.Position);


            if (nearbyFence != null)
            {
                if (InputManager.KeyPressed(Keys.E)) // ตรวจเฉพาะตอนกดครั้งเดียว
                {
                    nearbyFence.HitWithAxe(0.2f); // เพิ่มนิดเพื่อให้มี progress
                    interactionProgress += 0.2f;

                    if (interactionProgress >= interactionTime)
                    {
                        nearbyFence.Break();
                        interactionProgress = 0f;
                    }
                }
            }
            else
            {
                interactionProgress = 0f;
            }



            // ตรวจโจมตีศัตรูด้วย mouse
            Vector2 mousePosWorld = InputManager.MouseWorldPos(Camera.GetTransform(gm.root, gt));
            if (Mouse.GetState().LeftButton == ButtonState.Pressed && attackTimer <= 0f)
            {
                Attack(mousePosWorld);
                attackTimer = AttackCooldown;
            }
        }


        void Attack(Vector2 mousePosWorld)
        {
            Vector2 playerPos = gm.Player.Position;
            Vector2 dir = mousePosWorld - playerPos;
            if (dir != Vector2.Zero) dir.Normalize();

            float attackRange = 60f; // ระยะโจมตี

            // ตรวจศัตรู
            var enemy = gm.Level.CheckEnemyCollision(playerPos + dir * attackRange);
            if (enemy != null)
            {
                enemy.TakeDamage(Damage); // damage = 10
                enemy.WasHit = true;      // stun effect
            }
        }

        public void Draw(SpriteBatch sb)
        {

        }

    }
}
