﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioTest : MonoBehaviour
{
    [SerializeField] private AudioClip JumpOnGrass;
    [SerializeField] private AudioClip JumpOnRocks;
    [SerializeField] private AudioClip MoveOnGrass;
    [SerializeField] private AudioClip MoveOnRocks;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            AudioManager.Instance.PlaySfx(JumpOnGrass, 1);

        if (Input.GetKeyDown(KeyCode.W))
            AudioManager.Instance.PlayMusic(MoveOnGrass);

        if (Input.GetKeyUp(KeyCode.W))
            AudioManager.Instance.StopMusic(MoveOnGrass);
    }
}
