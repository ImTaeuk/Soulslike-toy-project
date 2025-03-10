using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private Player player;

    public PlayerInputSystem InputSystem { get; private set; }
    public Player Player => player;

    private void Awake()
    {
        Instance = this;

        InputSystem = this.GetComponent<PlayerInputSystem>();
    }

    private void Update()
    {
        if (Instance == null)
            Instance = this;
    }
}
