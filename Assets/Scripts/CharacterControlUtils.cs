using System;
using UnityEngine;

public static class CharacterControlUtils
{
    public static Vector3 GetNormalWithSphereCast(CharacterController characterController, LayerMask layer = default)
    {
        Vector3 normal = Vector3.up;
        Vector3 center = characterController.transform.position + characterController.center;
        float distance = characterController.height / 2f + characterController.stepOffset + 0.01f;
        RaycastHit hit;
        if (Physics.SphereCast(center, characterController.radius, Vector3.down, out hit, distance, layer))
        {
            normal = hit.normal;
        }
        return normal;
    }
}
