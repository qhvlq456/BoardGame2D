using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Res_2D_BoardGame;

public class StoneUI : MonoBehaviour
{
    [SerializeField]
    Button[] stoneButton;
    public int selectStone {get; private set; }
    private void Awake() {
        selectStone = (int)EPlayerType.white;
    }
    private void OnEnable() {
        stoneButton[selectStone].image.color = Color.green;
    }
    public void OnClickStoneSelectButton(int stone)
    {
        for(int i = 0; i < stoneButton.Length; i++)
        {
            if(i == stone)
            {
                stoneButton[i].image.color = Color.green;
            }
            else
            {
                stoneButton[i].image.color = Color.white;
            }
        }
        selectStone = stone;
    }
}
