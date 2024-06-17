using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDraggable
{
    void OnClick();
    void OnStartDrag();
    void OnEndDrag();
}