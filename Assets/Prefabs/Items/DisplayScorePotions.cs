using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DisplayScorePotions : MonoBehaviour
{
    [SerializeField] private GameObject damageText;
    [SerializeField] private GameObject scoreText;

    [SerializeField] private bool healthPotion = false;

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            ShowText(scoreText, 25);
            if (healthPotion)
            {
                ShowText(damageText, 25);
            }
        }
    }

    private void ShowText(GameObject textObject, int input)
    {
        if (textObject)
        {   
            GameObject prefab = Instantiate(textObject, transform.position, Quaternion.identity);
            if (healthPotion && textObject == damageText)
            {
                prefab.GetComponentInChildren<TextMeshPro>().text = "<color=#B0001C>" + input.ToString() + "</color>";
                return;
            }
            prefab.GetComponentInChildren<TextMeshPro>().text = input.ToString();
        }
    }
}
