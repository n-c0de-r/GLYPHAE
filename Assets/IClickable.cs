using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IClickable
{
    void OnClick();
    void OnStartDrag();
    void OnEndDrag();
}