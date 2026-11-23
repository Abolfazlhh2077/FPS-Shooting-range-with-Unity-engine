using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AmmoUI : MonoBehaviour
{


    public Text text;
    public PlayerInput playerInput;

    // Start is called before the first frame update
    void Start()
    {
        text.text = playerInput.clipRounds.ToString() + "/" + playerInput.totalAmmo.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        text.text = playerInput.clipRounds.ToString() + "/" + playerInput.totalAmmo.ToString();
    }


}
