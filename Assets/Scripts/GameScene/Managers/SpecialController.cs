using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SpecialController : MonoBehaviour
{
    public GameObject specialIcon, healthIcon, protectIcon, fullammoIcon, nukeIcon;
    public TextMeshProUGUI text;
    private Image contImg, iconImg;
    public static SpecialController instance;

    private float alpha = 0.4f;
    private string special;

    // Start is called before the first frame update
    void Start()
    {
        instance = this; 
    }

    // Update is called once per frame
    void Update()
    {
        if(Game.inGame) {
            int points = Game.currentPlayerScript.specialPoints;
            if(special == "Protect") { 
                if(points >= 8) {
                    ChangeAlpha(true);
                }
                else {
                    ChangeAlpha(false);
                }
            } 
            else if(special == "Health") { 
                if(points >= 10) {
                    ChangeAlpha(true);
                }
                else {
                    ChangeAlpha(false);
                }
            }
            else if(special == "FullAmmo") { 
                if(points >= 8) {
                    ChangeAlpha(true);
                }
                else {
                    ChangeAlpha(false);
                }
            }
            else if(special == "Nuke") { 
                if(points >= 12) {
                    ChangeAlpha(true);
                }
                else {
                    ChangeAlpha(false);
                }
            }
        }
    }

    public void StartSpecial() {
        special = Game.special;
        if(special == "") {
            specialIcon.SetActive(false);
            return;
        }
        specialIcon.SetActive(true);
        instance.contImg = instance.specialIcon.GetComponent<Image>();
        
        instance.ChangeBackColor(special);
        instance.specialIcon.SetActive(true);
        instance.ChangeIcon(special);
    }

    private void ChangeBackColor(string s) {
        if(s == "Protect") {
            contImg.color = new Color32(33,87,154,255);
        }
        else if(s == "Health") {
            contImg.color = new Color32(154,33,39,255);
        }
        else if(s == "FullAmmo") {
            contImg.color = new Color32(118,33,154,255);
        }
        else if(s == "Nuke") {
            contImg.color = new Color32(207,197,0,255);
        }
    }

    private void ChangeIcon(string s) {
        if(s == "Protect") {
            healthIcon.SetActive(false);
            protectIcon.SetActive(true);
            fullammoIcon.SetActive(false);
            nukeIcon.SetActive(false);

            iconImg = protectIcon.GetComponent<Image>();
        }
        else if(s == "Health") {
            healthIcon.SetActive(true);
            protectIcon.SetActive(false);
            fullammoIcon.SetActive(false);
            nukeIcon.SetActive(false);

            iconImg = healthIcon.GetComponent<Image>();
        } 
        else if(s == "FullAmmo") {
            healthIcon.SetActive(false);
            protectIcon.SetActive(false);
            fullammoIcon.SetActive(true);
            nukeIcon.SetActive(false);

            iconImg = fullammoIcon.GetComponent<Image>();
        }
        else if(s == "Nuke") {
            healthIcon.SetActive(false);
            protectIcon.SetActive(false);
            fullammoIcon.SetActive(false);
            nukeIcon.SetActive(true);

            iconImg = nukeIcon.GetComponent<Image>();
        }
    }

    private void ChangeAlpha(bool change) {
        if(change) {
            var tempColor = contImg.color;
            tempColor.a = 1f;
            contImg.color = tempColor;

            tempColor = iconImg.color;
            tempColor.a = 1f;
            iconImg.color = tempColor;

            tempColor = text.color;
            tempColor.a = 1f;
            text.color = tempColor;
        } else {
            var tempColor = contImg.color;
            tempColor.a = alpha;
            contImg.color = tempColor;

            tempColor = iconImg.color;
            tempColor.a = alpha;
            iconImg.color = tempColor;

            tempColor = text.color;
            tempColor.a = alpha;
            text.color = tempColor;
        }
    }

    public void Reset() {
        instance.specialIcon.SetActive(false);
        instance.healthIcon.SetActive(false);
        instance.protectIcon.SetActive(false);
        instance.fullammoIcon.SetActive(false);
        instance.nukeIcon.SetActive(true);
    }
}
