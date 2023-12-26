using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using moonNest;

public class BaseSwapWindow<T> : BaseWindow
{
    public Action<T, T> onSwapClicked, onMoveClicked;
    public Func<T, string> onGetName;

    private readonly List<SwapObject> swapObjects = new List<SwapObject>();
    private readonly TableDrawer<SwapObject> table;
    private bool disabled;


    public BaseSwapWindow() : base(100, "Swap Level Window")
    {
        winRect = new Rect(10, 10, 400, 400);

        table = new TableDrawer<SwapObject>();
        table.AddCol("Name", 200, ele => Draw.Label(onGetName?.Invoke(ele.t), 200));
        table.AddCol("    ", 40, ele =>
        {
            Draw.BeginDisabledGroup(disabled && !ele.selected);
            ele.selected = Draw.Toggle(ele.selected, 40);
            Draw.EndDisabledGroup();
        });
        table.drawOrder = false;
        table.drawDeleteButton = false;
        table.allowSwap = false;
        table.drawControl = false;
    }

    public bool Move { get; internal set; }

    public void SetUp(List<T> list)
    {
        int diff = swapObjects.Count - list.Count;
        if(diff > 0)
        {
            for(int i = 0; i < diff; i++) swapObjects.Remove(swapObjects.Last());
        }
        else if(diff < 0)
        {
            for(int i = 0; i < Math.Abs(diff); i++) swapObjects.Add(new SwapObject());
        }

        list.ForEach((ele, i) => swapObjects[i].t = ele);
    }

    protected override void OnDraw()
    {
        Draw.LabelBold("Select element to swap");
        Draw.Space(18);

        List<SwapObject> selecteds = swapObjects.FindAll(ele => ele.selected);
        disabled = selecteds.Count >= 2;
        Draw.BeginHorizontal();
        Draw.FitLabel(Move ? "Move " : "Swap ");
        if(selecteds.Count > 0)
        {
            Draw.FitLabelBold(onGetName?.Invoke(selecteds[0].t));
            Draw.FitLabel(Move ? " Before " : "   <->   ");
            if(selecteds.Count > 1) Draw.FitLabelBold(onGetName?.Invoke(selecteds[1].t));
        }
        Draw.FlexibleSpace();
        Draw.EndHorizontal();

        Draw.Space(12);
        table.DoDraw(swapObjects);
        Draw.Space(12);

        Draw.BeginDisabledGroup(selecteds.Count < 2);
        Draw.BeginHorizontal();
        {
            Draw.Space(12);
            if(Draw.Button(Move ? "Move" : "Swap", 120))
            {
                if(Move)
                {
                    onMoveClicked?.Invoke(selecteds[0].t, selecteds[1].t);
                }
                else
                {
                    onSwapClicked?.Invoke(selecteds[0].t, selecteds[1].t);

                    int index1 = swapObjects.IndexOf(selecteds[0]);
                    int index2 = swapObjects.IndexOf(selecteds[1]);

                    var temp = swapObjects[index1];
                    swapObjects[index1] = swapObjects[index2];
                    swapObjects[index2] = temp;

                    swapObjects[index1].selected = false;
                    swapObjects[index2].selected = false;
                }
            }
            Draw.EndDisabledGroup();
        }
        Draw.EndHorizontal();
        Draw.Space(12);

        winRect.width = 420 + (Math.Max(0, table.Pages - 6) * 35);
    }

    public class SwapObject
    {
        public bool selected = false;
        public T t;
    }
}