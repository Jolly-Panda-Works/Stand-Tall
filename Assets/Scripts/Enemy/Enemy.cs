using JollyPanda.LastFlag.Handlers;
using System;
using UnityEngine;

namespace JollyPanda.LastFlag.EnemyModule
{
    public class Enemy : MonoBehaviour
    {
        [SerializeField] private EnemyAnimationController animationController;
        [SerializeField] private EnemyHealth health;
        [SerializeField] private EnemyMovement movement;
        [SerializeField] private EnemyPositionChecker positionChecker;
        [SerializeField] private EnemySoundController soundController;

        public int CurrentSpawnIndex { get; set; }

        private Action<Enemy> OnDead;
        private Action<Enemy> OnReach;

        private void OnEnable()
        {
            health.OnDead += Falling;
            //movement.OnStartClimbing += Climbing;
            //movement.OnStartFalling += Falling;
            positionChecker.OnEnemyReachedTop += ReachingTop;
            positionChecker.OnEnemyReachedGround += ReachingGround;
            Informant.OnEnemyReachedTop += OneEnemyReachedTop;
            Informant.OnPause += PauseAnything;

            // an enemy disabled in the middle of a pause must not keep animator.speed = 0
            animationController.Resume();

            // Start climbing from here instead of relying on EnemyHealth.OnEnable -> OnRevive,
            // which only worked when EnemyHealth was enabled after this component.
            Climbing();
        }

        private void OnDisable()
        {
            health.OnDead -= Falling;
            //movement.OnStartClimbing -= Climbing;
            //movement.OnStartFalling -= Falling;
            positionChecker.OnEnemyReachedTop -= ReachingTop;
            positionChecker.OnEnemyReachedGround -= ReachingGround;
            Informant.OnEnemyReachedTop -= OneEnemyReachedTop;
            Informant.OnPause -= PauseAnything;
        }

        private void Climbing()
        {
            movement.Climbing();
            animationController.PlayClimb();
            positionChecker.SetCheckState(EnemyPositionChecker.CheckState.MoveUp);
            soundController.PlayClimbSound();
        }

        private void Falling()
        {
            movement.Falling();
            animationController.PlayFall();
            positionChecker.SetCheckState(EnemyPositionChecker.CheckState.MoveDown);
            health.ShowHealthBar(false);
            soundController.PlayFallSound();
            OnDead?.Invoke(this);
        }

        private void ReachingTop()
        {
            movement.StopClimbing();
            health.ShowHealthBar(false);
            animationController.PlayClimbingOverEdge();
            OnReach?.Invoke(this);
        }

        private void OneEnemyReachedTop(Enemy reachedEnemy)
        {
            if (reachedEnemy == this)
                return;

            movement.StopClimbing();
            animationController.PlayHangingIdle();
            health.ShowHealthBar(false);
        }

        private void ReachingGround()
        {
            gameObject.SetActive(false);
        }

        public void SetClimbSpeed(float value)
        {
            movement.SetClimbSpeed(value);
        }

        public void SetDeadAction(Action<Enemy> action)
        {
            OnDead = action;
        }

        public void SetReachAction(Action<Enemy> action)
        {
            OnReach = action;
        }

        private void PauseAnything(bool isPause)
        {
            movement.SetPaused(isPause);

            if (isPause)
                animationController.Pause();
            else
                animationController.Resume();
        }

        internal void SetHealth(int value)
        {
            health.SetMaxHealth(value);
        }
    }
}
