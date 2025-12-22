using System.Linq;
using UnityEngine;

public class RandomState : StateMachineBehaviour
{
    [SerializeField] private string key;
    private static string[] animations;
    private static bool isInitialized = false; 

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //if (!isInitialized)
        //{
        //    InitializeIdleAnimations(animator);
        //    isInitialized = true;
        //}

        //if (animations.Length > 0)
        //{
        //    string randomIdle = animations[Random.Range(0, animations.Length)];
        //    animator.Play(randomIdle);
        //}
    }

    private void InitializeIdleAnimations(Animator animator)
    {
        //var controller = animator.runtimeAnimatorController as AnimatorController;

        //var animationClips = controller.animationClips;

        //animations = System.Array.FindAll(animationClips, clip => clip.name.Contains(key))
        //                         .Select(clip => clip.name)
        //                         .ToArray();
    }
}
