using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadialMenuMB : MonoBehaviour
{
    public RadialMenuRing Data;
    public RadialMenuCakePiece CakePiecePrefab;
    public float GapWidthDegree = 1f;
    public Action<string> callback;
    protected RadialMenuCakePiece[] Pieces;
    protected RadialMenuMB Parent;
    [HideInInspector] public string Path;

    private Color32 baseColourBackground = new Color32(0, 0, 0, 110);
    private Color32 hoverColourBackground = new Color32(0, 0, 0, 175);
    private Color32 baseColourIcon = new Color32(184, 184, 184, 255);
    private Color32 hoverColourIcon = new Color32(255, 255, 255, 255);

    void Start()
    {
        var stepLength = 360f / Data.Elements.Length;
        var iconDist = Vector3.Distance(CakePiecePrefab.Icon.transform.position, CakePiecePrefab.CakePiece.transform.position);

        // Positioning
        Pieces = new RadialMenuCakePiece[Data.Elements.Length];
        for (int i = 0; i < Data.Elements.Length; i++)
        {
            // Set Up Root Element
            Pieces[i] = Instantiate(CakePiecePrefab, transform);
            Pieces[i].transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

            // Set Up Cake Piece
            Pieces[i].CakePiece.fillAmount = 1f / Data.Elements.Length - GapWidthDegree / 360f;
            Pieces[i].CakePiece.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.Euler(0, 0, stepLength / 2f + GapWidthDegree / 2f + i * stepLength));

            // Set Up Icon ~ Float being added to 'iconDist' at end of line will change how far icon is from its origin
            Pieces[i].Icon.transform.localPosition = Pieces[i].CakePiece.transform.localPosition + Quaternion.AngleAxis(i * stepLength, Vector3.forward) * Vector3.up * (iconDist + 40f);
            Pieces[i].Icon.sprite = Data.Elements[i].Icon;
            Pieces[i].Icon.transform.localScale = new Vector3(1.25f, 1.25f, 1.25f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        var stepLength = 360f / Data.Elements.Length;
        var mouseAngle = NormalizeAngle(Vector3.SignedAngle(Vector3.up, Input.mousePosition - new Vector3(Screen.width / 2, Screen.height / 2), Vector3.forward) + stepLength / 2f);
        var activeElement = (int)(mouseAngle / stepLength);
        for (int i = 0; i < Data.Elements.Length; i++)
        {
            if (i == activeElement)  /* If the mouse is on this element */
            {
                Pieces[i].CakePiece.color = hoverColourBackground;
                Pieces[i].Icon.color = hoverColourIcon;
            }
            else /* All elements the mouse is not on */
            {
                Pieces[i].CakePiece.color = baseColourBackground;
                Pieces[i].Icon.color = baseColourIcon;
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (Data.Elements[activeElement].SelectionAvailable == false)  // If selection is not turned on for the items scriptable object
                return;

            var path = Path + "/" + Data.Elements[activeElement].Name;
            if (Data.Elements[activeElement].NextRing != null)
            {
                var newSubRing = Instantiate(gameObject, transform.parent).GetComponent<RadialMenuMB>();
                newSubRing.Parent = this;
                for (var j = 0; j < newSubRing.transform.childCount; j++)
                    Destroy(newSubRing.transform.GetChild(j).gameObject);
                newSubRing.Data = Data.Elements[activeElement].NextRing;
                newSubRing.Path = path;
                newSubRing.callback = callback;
            }
            else
            {
                callback?.Invoke(path);
            }
            gameObject.SetActive(false);
        }
    }
    private float NormalizeAngle(float angle) => (angle + 360f) % 360f;
}
