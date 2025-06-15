using UnityEngine;
//using Invector.vCharacterController;

public class PlayerAiming : PlayerAbstract
{
    [SerializeField] protected bool isAlwaysAiming = false;
    protected float closeLookDistance = 0.6f;
    protected float farLookDistance = 1.3f;

    private void Update()
    {
        this.Aiming();
    }

    protected virtual void Aiming()
    {
        if (this.isAlwaysAiming || InputManager.Instance.IsAiming()) this.LookClose();
        else this.LookFar();
    }

    protected virtual void LookClose()
    {
        this.playerCtrl.ThirdPersonCamera.defaultDistance = this.closeLookDistance;

        this.playerCtrl.ThirdPersonController.lockRotation = true;

        CrosshairPointer crosshairPointer = this.playerCtrl.CrosshairPointer;
        this.playerCtrl.ThirdPersonController.RotateToPosition(crosshairPointer.transform.position);

        this.playerCtrl.ThirdPersonController.isSprinting = false;

        this.playerCtrl.AimingRig.weight = 1f;
    }

    protected virtual void LookFar()
    {
        this.playerCtrl.ThirdPersonCamera.defaultDistance = this.farLookDistance;
        this.playerCtrl.ThirdPersonController.lockRotation = false;

        this.playerCtrl.AimingRig.weight = 0f;
    }
}
