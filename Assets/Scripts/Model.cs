using System;
using System.Collections.Generic;
using UnityEngine;

public static class Model
{
    #region Player

    [Serializable]
    public class PlayerSettings
    {
        [Header("Sensitivity")]
        public float senX;
        public float senY;

        public bool invertX;
        public bool invertY;

        [Header("Movement Speed")]
        public float walkForward;
        public float walkBackward;
        public float walkStrafe;
        public float movementSmooth;
        
        [Header("Running Speed")]
        public float runForward;
        public float runStrafe;
        public bool runHold;

        [Header("Jump")]
        public float jumpHeight;
        public float jumpFall;
        public float fallSmooth;        
        
        [Header("SpeedEffector")]
        public float speedEffect;
        public float fallSpeedEffect;

        [Header("Ground Check")]
        public float groundRadius;
        public float fallSpeed;
    }

    #endregion

    #region Weapon

    [Serializable]
    public class WeaponSettings
    {
        [Header("Sway")]
        public float swayAmount;
        public float swaySmooth;
        public float resetSmooth;

        public float swayXClamp;
        public float swayYClamp;

        public bool invertXSway;
        public bool invertYSway;        
        
        [Header("Movement Sway")]
        public float swayMovementX;
        public float swayMovementY;
        public float swayMovementSmooth;

        public bool invertXMovementSway;
        public bool invertYMovementSway;
    }

    #endregion
}
