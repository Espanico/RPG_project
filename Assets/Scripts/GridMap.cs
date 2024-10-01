using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Playables;
using UnityEngine;

public class GridMap<TGridObject>
{
    public event EventHandler<OnGridValueChangedEventArgs> OnGridValueChanged;
    public class OnGridValueChangedEventArgs : EventArgs {
        public int x;
        public int y;
    }


    private int width;
    private int height;
    public float cellSize;
    public Vector3 originPosition;
    private TGridObject[,] gridArray;

    public GridMap(int width, int height, float cellSize, Vector3 originPosition, Func<GridMap<TGridObject>, int, int, TGridObject> createGridObject) {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.originPosition = originPosition;

        gridArray = new TGridObject[width, height];

        for(int x = 0; x < gridArray.GetLength(0); x++) {
            for(int y = 0; y < gridArray.GetLength(1); y++) {
                gridArray[x, y] = createGridObject(this, x, y);

                //Debug.Log(x*cellSize + ", " + y*cellSize);                                                      // DA COMMENTARE
                Debug.DrawLine(GetWorldPosition(x, y)-Vector3.one*cellSize*.5f, GetWorldPosition(x, y+1)-Vector3.one*cellSize*.5f, Color.white, 100f, false);     // DA COMMENTARE
                Debug.DrawLine(GetWorldPosition(x, y)-Vector3.one*cellSize*.5f, GetWorldPosition(x+1, y)-Vector3.one*cellSize*.5f, Color.white, 100f, false);     // DA COMMENTARE
            }
        }
        Debug.DrawLine(GetWorldPosition(0, height)-Vector3.one*cellSize*.5f, GetWorldPosition(width, height)-Vector3.one*cellSize*.5f, Color.white, 100f, false); // DA COMMENTARE
        Debug.DrawLine(GetWorldPosition(width, 0)-Vector3.one*cellSize*.5f, GetWorldPosition(width, height)-Vector3.one*cellSize*.5f, Color.white, 100f, false);  // DA COMMENTARE
    }

    private Vector3 GetWorldPosition(int x, int y) {
        return new Vector3(x, y) * cellSize + originPosition;
    }

    public void SetGridObject(int x, int y, TGridObject value) {
        if(x >= 0 && y >= 0 && x < width && y < height) {
            gridArray[x, y] = value;
            if(OnGridValueChanged != null) OnGridValueChanged(this, new OnGridValueChangedEventArgs {x=x, y=y});
        }
    }

    public void TriggerGridObjectChanged(int x, int y) {
        if(OnGridValueChanged != null) OnGridValueChanged(this, new OnGridValueChangedEventArgs {x=x, y=y});
    }

    public void GetXY(Vector3 worldPosition, out int x, out int y) {
        x = Mathf.FloorToInt(Mathf.Round((worldPosition-originPosition).x*100)/100 / cellSize);
        y = Mathf.FloorToInt(Mathf.Round((worldPosition-originPosition).y*100)/100 / cellSize);
    }

    public void SetGridObject(Vector3 worldPosition, TGridObject value) {
        int x, y;
        GetXY(worldPosition, out x, out y);
        SetGridObject(x, y, value);
    }

    public TGridObject GetGridObject(int x, int y) {
        if(x >= 0 && y >= 0 && x < width && y < height) {
            return gridArray[x, y];
        } else {
            Debug.Log("MALEMALE");
            return default(TGridObject);
        }
    }

    public TGridObject GetGridObject(Vector3 worldPosition) {
        int x, y;
        GetXY(worldPosition, out x, out y);
        return GetGridObject(x, y);
    }

    public int GetWidth() {
        return width;
    }

    public int GetHeight() {
        return height;
    }
}
