using UnityEditor.Animations;
using UnityEngine;

public class WoodTower : MonoBehaviour
{
    [Header("Wood Tower Skins")]
    public AnimatorController[] animatorControllers;

    public WoodTowerSkin selectedSkin;

    public enum WoodTowerSkin
    {
        Blue,
        Purple,
        Red,
        Yellow
    }

    Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        applySkin();
    }

    public void applySkin()
    {
        int skinIndex = (int)selectedSkin;
        if(animatorControllers != null && skinIndex < animatorControllers.Length)
        {
            animator.runtimeAnimatorController = animatorControllers[skinIndex];
        }
        else
        {
            Debug.LogWarning($"WoodTower: Invalid skin index {skinIndex} or animatorControllers not set properly.");
        }
    }
}
