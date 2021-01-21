using System;
using System.Numerics;
using MMORPG_Server.Main;
using MMORPG_Server.Network.Senders;

namespace MMORPG_Server.ClientEntity {
    public class Player {
        public int Id { get; }
        public string Username { get; }
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; private set; }
        private float moveSpeed = 5f / ServerConfigConstants.TICKS_PER_SECOND;
        public bool[] Inputs { get; set; } = new bool[4];

        public Player(int id, string username, Vector3 position) {
            Id = id;
            Username = username;
            Position = position;
            Rotation = Quaternion.Identity;
        }

        public void Update() {
            Vector2 inputDirection = Vector2.Zero;
            bool sendPackage = false;
            foreach (bool input in Inputs) {
                if(input)
                    Console.WriteLine(input);
            }
            if (Inputs[0]) {
                sendPackage = true;
                inputDirection.Y += 1;
            }
            if (Inputs[1]) {
                sendPackage = true;
                inputDirection.Y -= 1;
            }
            if (Inputs[2]) {
                sendPackage = true;
                    inputDirection.X += 1;
            }
            if (Inputs[0]) {
                sendPackage = true;
                    inputDirection.X -= 1;
            }
            if(sendPackage)
                Move(inputDirection);
        }

        private void Move(Vector2 inputDirection) {
            Vector3 forward = Vector3.Transform(new Vector3(0, 0, 1), Rotation);
            Vector3 right = Vector3.Normalize(Vector3.Cross(forward, new Vector3(0,1,0)));
            Vector3 moveDirection = right * inputDirection.X + forward * inputDirection.Y;
            Position += moveDirection * moveSpeed;

            ServerSend.PlayerPosition(this);
            ServerSend.PlayerRotation(this);
        }

        public void SetInput(bool[] inputs, Quaternion rotation) {
            Inputs = inputs;
            Rotation = rotation;
        }
    }
}