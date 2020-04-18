using System.Linq;
using Box2DX.Common;
using Box2DX.Dynamics;
using TDOS.Box2D.Skia;

namespace TDOS.Game.Characters
{
    public class Character
    {
        public Character(Body body)
        {
            Body = body;
        }

        public Body Body { get; }

        public float Acceleration { get; set; }

        public float MaxSpeed
        {
            get => _maxSpeed;
            set
            {
                _maxSpeed = value;
                maxSpeedSquared = _maxSpeed * _maxSpeed;
            }
        }

        public void UpdatePosition(params MoveDirection[] activeDirections)
        {
            var cappedForce = CreateForceCappedToAcceleration(
                GetXForce(activeDirections),
                GetYForce(activeDirections));
            var velocity = Body.GetLinearVelocity();

            if (velocity.LengthSquared() > maxSpeedSquared)
            {
                var normalizedVelocity = Body.GetNormalizedLinearVelocity();
                var reductionForce = normalizedVelocity * cappedForce.Length();

                Body.ApplyForce(cappedForce - reductionForce, Body.GetWorldCenter());
            }
            else
            {
                Body.ApplyForce(cappedForce, Body.GetWorldCenter());
            }
        }

        public void UpdateRotation(Vec2 mouseWorldPosition)
        {
            var position = Body.GetPosition();
            var destinationAngle = System.Math.Atan2(
                mouseWorldPosition.Y - position.Y,
                mouseWorldPosition.X - position.X);

            Body.SetXForm(Body.GetPosition(), (float)destinationAngle);
        }

        private Vec2 CreateForceCappedToAcceleration(float x, float y)
        {
            if (System.Math.Abs(x) < Epsilon && System.Math.Abs(y) < Epsilon) return Vec2.Zero;

            var force = new Vec2(x, y);
            force.Normalize();
            force *= Acceleration;

            return force;
        }

        private float GetXForce(MoveDirection[] activeDirections)
            => GetDirectionalForce(activeDirections, MoveDirection.Left, MoveDirection.Right);

        private float GetYForce(MoveDirection[] activeDirections)
            => GetDirectionalForce(activeDirections, MoveDirection.Up, MoveDirection.Down);

        private float GetDirectionalForce(
            MoveDirection[] activeDirections,
            MoveDirection negativeDirection,
            MoveDirection positiveDirection)
        {
            if (activeDirections.Contains(negativeDirection)
              && !activeDirections.Contains(positiveDirection))
            {
                return (-1f) * Acceleration;
            }
            else if (activeDirections.Contains(positiveDirection)
                && !activeDirections.Contains(negativeDirection))
            {
                return Acceleration;
            }

            return 0f;
        }

        private const float Epsilon = 1e-6f;

        private float _maxSpeed;

        private float maxSpeedSquared;
    }
}
