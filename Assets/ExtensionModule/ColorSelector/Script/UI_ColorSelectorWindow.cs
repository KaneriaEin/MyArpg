﻿using JKFrame;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[UIWindowData(nameof(UI_ColorSelectorWindow), true, nameof(UI_ColorSelectorWindow), 4)]
public class UI_ColorSelectorWindow : UI_WindowBase{
	public Transform handleImage,outerCursor,innerCursor;
	public Button CloseButton;
	Color finalColor, selectedColor;
	float selectorAngle=0.0f;
	Vector2 innerDelta=Vector2.zero;
	private float scale;
    public override void Init()
    {
		base.Init();
		scale = handleImage.GetComponent<RectTransform>().sizeDelta.x / 200f;
		handleImage.OnClick(OnPointerClick);
		handleImage.OnDrag(OnPointerClick);
		CloseButton.onClick.AddListener(Close);
		selectedColor = Color.white;
		SelectOuterColor(outerCursor.position);
		SelectInnerColor(Vector2.zero);
	}

    public void Close()
    {
        UISystem.Close<UI_ColorSelectorWindow>();
    }

    public override void OnClose()
    {
        base.OnClose();
		OnColorSelected = null;
	}
    private System.Action<Color> OnColorSelected;
    public void Init(System.Action<Color> onColorSelected,Color color)
    {
		OnColorSelected = onColorSelected;
		selectedColor = color;
	}

	private void OnPointerClick(PointerEventData eventData)
	{
		Vector3 localPosition = transform.InverseTransformPoint(eventData.position);
		float dist = Vector2.Distance(Vector2.zero, localPosition);

		if (dist > 78* scale)
			SelectOuterColor(localPosition);
		else
			SelectInnerColor(localPosition);
		OnColorSelected?.Invoke(GetColor());
	}

	void SelectInnerColor(Vector2 delta){
		float v=0.0f, w=0.0f, u=0.0f;
		Barycentric (delta,ref v,ref w,ref u);
		if (v >= 0.15f && w >= -0.15f && u >= -0.15f) {
			Vector3 colorVector = new Vector3 (selectedColor.r, selectedColor.g, selectedColor.b);
			Vector3 finalColorVector = v * colorVector + u * new Vector3 (0.0f, 0.0f, 0.0f) + w * new Vector3 (1.0f, 1.0f, 1.0f);
			finalColor = new Color (finalColorVector.x, finalColorVector.y, finalColorVector.z);
			innerCursor.localPosition =delta;
			innerDelta = delta;
		}
	}
	Vector3 ClampPosToCircle(Vector3 pos){
		Vector3 newPos = Vector3.zero;
		float dist = 90*scale;
		float angle = Mathf.Atan2(pos.x, pos.y);// * 180 / Mathf.PI;
		newPos.x = dist * Mathf.Sin( angle ) ;
		newPos.y = dist * Mathf.Cos( angle ) ;
		newPos.z = pos.z;
		return newPos;
	}

	void Barycentric(Vector2 point,ref float u,ref float v,ref float w){

		Vector2 a = new Vector2 (0.0f, 51.7f*scale);
		Vector2 b = new Vector2 (-51.7f * scale, -51.7f * scale);
		Vector2 c = new Vector2 (51.7f * scale, -51.7f * scale);
		Vector2 v0 = b - a, v1 = c - a, v2 = point - a;
		float d00 = Vector2.Dot(v0, v0);
		float d01 = Vector2.Dot(v0, v1);
		float d11 = Vector2.Dot(v1, v1);
		float d20 = Vector2.Dot(v2, v0);
		float d21 = Vector2.Dot(v2, v1);
		float denom = d00 * d11 - d01 * d01;
		v = (d11 * d20 - d01 * d21) / denom;
		w = (d00 * d21 - d01 * d20) / denom;
		u = 1.0f - v - w;
	}
	void SelectOuterColor(Vector2 delta){
		float angle= Mathf.Atan2(delta.x, delta.y);
		float angleGrad=angle*57.2957795f;
		if(angleGrad<0.0f)
			angleGrad=360+angleGrad;
		selectorAngle=angleGrad/360;
		selectedColor=HSVToRGB(selectorAngle,1.0f,1.0f);
		handleImage.GetComponent<UnityEngine.UI.Image>().material.SetColor("_Color",selectedColor);
		outerCursor.transform.localPosition = ClampPosToCircle (delta);/// delta*0.75f;
		SelectInnerColor (innerDelta);

	}
	public static Color HSVToRGB(float H, float S, float V)
	{
		if (S == 0f)
			return new Color(V,V,V);
		else if (V == 0f)
			return Color.black;
		else
		{
			Color col = Color.black;
			float Hval = H * 6f;
			int sel = Mathf.FloorToInt(Hval);
			float mod = Hval - sel;
			float v1 = V * (1f - S);
			float v2 = V * (1f - S * mod);
			float v3 = V * (1f - S * (1f - mod));
			switch (sel + 1)
			{
			case 0:
				col.r = V;
				col.g = v1;
				col.b = v2;
				break;
			case 1:
				col.r = V;
				col.g = v3;
				col.b = v1;
				break;
			case 2:
				col.r = v2;
				col.g = V;
				col.b = v1;
				break;
			case 3:
				col.r = v1;
				col.g = V;
				col.b = v3;
				break;
			case 4:
				col.r = v1;
				col.g = v2;
				col.b = V;
				break;
			case 5:
				col.r = v3;
				col.g = v1;
				col.b = V;
				break;
			case 6:
				col.r = V;
				col.g = v1;
				col.b = v2;
				break;
			case 7:
				col.r = V;
				col.g = v3;
				col.b = v1;
				break;
			}
			col.r = Mathf.Clamp(col.r, 0f, 1f);
			col.g = Mathf.Clamp(col.g, 0f, 1f);
			col.b = Mathf.Clamp(col.b, 0f, 1f);
			return col;
		}
	}
	public Color GetColor(){
		return finalColor;
	}
}
