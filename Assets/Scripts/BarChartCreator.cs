using IATK;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class BarChartCreator : MonoBehaviour {

    // input data
    public List<TextAsset> datasets;

    public List<GameObject> barList;

    List<GameObject> axisList;

    CSVDataSource customCSVData;
    List<View> finalVs;
    Color[] OldMappedColors;
    Color[] UpdatedMappedColors;

    SmallMultiplesManagerScript smms;

    // brushing
    bool[] chessBoardBrushingBool = new bool[25];

    // Use this for initialization
    void Awake()
    {
        barList = new List<GameObject>();
        axisList = new List<GameObject>();
        finalVs = new List<View>();
        int count = 0;

        smms = GameObject.Find("SmallMultiplesManager").GetComponent<SmallMultiplesManagerScript>();

        datasets.Clear();
        for (int i = 1; i <= smms.smallMultiplesNumber; i++)
        {
            TextAsset file = (TextAsset)Resources.Load("bData" + i);
            datasets.Add(file);
        }
 
        foreach (TextAsset ta in datasets)
        {

            GameObject go = new GameObject("BarCharts-" + count);
            go.transform.parent = this.transform;

            CSVDataSource csvdata = createCSVDataSource(ta.text, go);
            customCSVData = csvdata;

            finalVs.Add(CreateBarchart(csvdata, go, count));
            count++;
            barList.Add(go);

        }


    }

    CSVDataSource createCSVDataSource(string data, GameObject go)
    {
        CSVDataSource dataSource;
        dataSource = go.AddComponent<CSVDataSource>();
        dataSource.load(data, null);
        return dataSource;
    }

    public Vector3 Spherical(float r, float theta, float phi)
    {
        Vector3 pt = new Vector3();
        float snt = (float)Mathf.Sin(theta * Mathf.PI / 180);
        float cnt = (float)Mathf.Cos(theta * Mathf.PI / 180);
        float snp = (float)Mathf.Sin(phi * Mathf.PI / 180);
        float cnp = (float)Mathf.Cos(phi * Mathf.PI / 180);
        pt.x = r * snt * cnp;
        pt.y = r * cnt;
        pt.z = -r * snt * snp;
        return pt;
    }

    public Color ColorTransform(int id) {
        float fc = 255f;

        switch (id)
        {
            //case 0:
            //    return new Color(215f / fc, 48f / fc, 39f / fc); 
            //case 1:
            //    return new Color(244f / fc, 109f / fc, 67f / fc);
            //case 2:
            //    return new Color(253f / fc, 174f / fc, 97f / fc); 
            //case 3:
            //    return new Color(254f / fc, 224f / fc, 144f / fc);
            //case 4:
            //    return new Color(255f / fc, 255f / fc, 191f / fc);
            //case 5:
            //    return new Color(224f / fc, 243f / fc, 248f / fc); 
            //case 6:
            //    return new Color(171f / fc, 217f / fc, 233f / fc); 
            //case 7:
            //    return new Color(116f / fc, 173f / fc, 209f / fc); 
            //case 8:
            //    return new Color(69f / fc, 117f / fc, 180f / fc); 
            //case 9:
            //    return new Color(49f / fc, 54f / fc, 149f / fc);
            case 0:
                return new Color(228f / fc, 26f / fc, 28f / fc);
            case 1:
                return new Color(55f / fc, 126f / fc, 184f / fc);
            case 2:
                return new Color(77f / fc, 175f / fc, 74f / fc);
            case 3:
                return new Color(152f / fc, 78f / fc, 163f / fc);
            case 4:
                return new Color(255f / fc, 127f / fc, 0f / fc);
            default:
                return Color.white;
        }
    }

    // a space time cube
    View CreateBarchart(CSVDataSource csvds, GameObject go, int count)
    {
        // header
        // Date,Time,Lat,Lon,Base
        Gradient g = new Gradient();
        GradientColorKey[] gck = new GradientColorKey[2];
        gck[0] = new GradientColorKey(Color.blue, 0);
        gck[1] = new GradientColorKey(Color.red, 1);
        g.colorKeys = gck;

        Color[] colourPalette = new Color[5];

        for (int i = 0; i < 5; i++) {
            colourPalette[i] = ColorTransform(i);
        }

        // create a view builder with the point topology
        ViewBuilder vb = new ViewBuilder(MeshTopology.Points, "BarCharts-" + count).
            initialiseDataView(csvds.DataCount).
            setDataDimension(csvds["Country"].Data, ViewBuilder.VIEW_DIMENSION.X).
            setDataDimension(csvds["Value"].Data, ViewBuilder.VIEW_DIMENSION.Y).
            setDataDimension(csvds["Year"].Data, ViewBuilder.VIEW_DIMENSION.Z);

      

        // initialise the view builder wiith thhe number of data points and parent GameOBject

        //Enumerable.Repeat(1f, dataSource[0].Data.Length).ToArray()
        Material mt = IATKUtil.GetMaterialFromTopology(AbstractVisualisation.GeometryType.Bars);
        mt.SetFloat("_MinSize", 4.5f); //0.01f
        mt.SetFloat("_MaxSize", 4.5f); //0.05f


        View v = vb.updateView().apply(go, mt);
        //v.name = "BarCharts-" + (count + 1);

        Color[] mappedColors = new Color[csvds.DataCount];

        float[] data = csvds["Country"].Data;
        float[] uniqueValues = data.Distinct().ToArray();

        for (int i = 0; i < data.Length; i++)
        {
            int indexColor = Array.IndexOf(uniqueValues, data[i]);
            mappedColors[i] = colourPalette[indexColor];
        }
        OldMappedColors = mappedColors;
        UpdatedMappedColors = mappedColors;


        v.SetColors(mappedColors);

        Visualisation visualisation = new Visualisation();
        visualisation.dataSource = csvds;

        //  Visualisation

        Vector3 globalScale = new Vector3(1, 1f, 1);


        Vector3 posx = Vector3.zero;
        posx.x = -0f;
        posx.y = -0.05f;
        posx.z = -0.2f;
        DimensionFilter xDimension = new DimensionFilter { Attribute = "Country" };
        GameObject X_AXIS = CreateAxis(AbstractVisualisation.PropertyType.X, xDimension, posx, new Vector3(0f, 0f, 0f), globalScale, 0, csvds, visualisation, go);
        axisList.Add(X_AXIS);

        Vector3 posy = Vector3.zero;
        posy.x = -0.2f;
        posy.z = -0.2f;
        DimensionFilter yDimension = new DimensionFilter { Attribute = "Value" };
        GameObject Y_AXIS = CreateAxis(AbstractVisualisation.PropertyType.Y, yDimension, posy, new Vector3(0f, 0f, 0f), globalScale, 1, csvds, visualisation, go);
        Y_AXIS.transform.localScale = new Vector3(1, 1.25f, 1);
        axisList.Add(Y_AXIS);
        Y_AXIS.transform.Find("AxisLabels").localEulerAngles = new Vector3(0, -30, 0);

        Vector3 posz = Vector3.zero;
        posz.x = -0.2f;
        posz.y = -0.05f;
        posz.y = -0.05f;
        DimensionFilter zDimension = new DimensionFilter { Attribute = "Year" };
        GameObject Z_AXIS = CreateAxis(AbstractVisualisation.PropertyType.Z, zDimension, posz, new Vector3(0f, 90f, 90f), globalScale, 2, csvds, visualisation, go);
        axisList.Add(Z_AXIS);

        // testing new axises
        posx.y = -0.05f;
        posx.z = 1.2f;
        xDimension = new DimensionFilter { Attribute = "Country" };
        X_AXIS = CreateAxis(AbstractVisualisation.PropertyType.X, xDimension, posx, new Vector3(0f, 0f, 0f), globalScale, 3, csvds, visualisation, go);
        axisList.Add(X_AXIS);

        posy.x = 1.2f;
        posy.z = 1.2f;
        yDimension = new DimensionFilter { Attribute = "Value" };
        Y_AXIS = CreateAxis(AbstractVisualisation.PropertyType.Y, yDimension, posy, new Vector3(0f, 0f, 0f), globalScale, 4, csvds, visualisation, go);
        Y_AXIS.transform.localScale = new Vector3(1, 1.25f, 1);
        axisList.Add(Y_AXIS);
        Y_AXIS.transform.Find("AxisLabels").localEulerAngles = new Vector3(0, 150, 0);

        posz.x = 1.2f;
        posz.y = -0.05f;
        zDimension = new DimensionFilter { Attribute = "Year" };
        Z_AXIS = CreateAxis(AbstractVisualisation.PropertyType.Z, zDimension, posz, new Vector3(0f, 90f, 90f), globalScale, 5, csvds, visualisation, go);
        axisList.Add(Z_AXIS);

        // end testing new axises

        foreach (GameObject axis in axisList) {
            axis.transform.Find("MinNormaliser").gameObject.SetActive(false);
            axis.transform.Find("MaxNormaliser").gameObject.SetActive(false);

            axis.transform.Find("Cone").localScale = new Vector3(0.04f, 0.8f, 0.04f);
            if (axis.name != "axis Value")
            {
                axis.transform.Find("Cone").localPosition = new Vector3(0, 1.12f, 0);
            }

            Transform axisLabels = axis.transform.Find("AxisLabels");
            foreach (Transform t in axisLabels) {
                TextMeshPro tmp = t.GetComponent<TextMeshPro>();
                tmp.fontSize = 0.8f;
                tmp.maskType = MaskingTypes.MaskSoft;
            }

            if (axis.name == "axis Country")
            {
                // assign colours
                //for (int i = 1; i < axis.transform.Find("AxisLabels").childCount; i++) {
                //    TextMeshPro tmp = axis.transform.Find("AxisLabels").GetChild(i).GetComponent<TextMeshPro>();
                //    tmp.color = ColorTransform(i - 1);
                //}

                // axis facing adjustment
                axis.transform.localEulerAngles = new Vector3(180, 0, -90);

                // attribute label adjustment
                Transform attributeLabel = axis.transform.Find("AttributeLabel");
                if (axis.transform.GetSiblingIndex() == 1)
                {
                    attributeLabel.localEulerAngles = new Vector3(-180, 0, -90);
                    // axis labels facing adjustment
                    for (int i = 1; i < axis.transform.Find("AxisLabels").childCount; i++)
                    {
                        axis.transform.Find("AxisLabels").GetChild(i).localEulerAngles = new Vector3(180, 0, 0);
                    }
                }
                else
                {
                    attributeLabel.localEulerAngles = new Vector3(180, 180, 90);
                }              
                TextContainer tc = attributeLabel.GetComponent<TextContainer>();
                tc.pivot = new Vector2(0.5f, 2);

                for (int i = 1; i < axis.transform.Find("AxisLabels").childCount; i++) {
                    axis.transform.Find("AxisLabels").GetChild(i).localPosition = new Vector3(0, 0.25f * (i - 1), 0);
                }
            }

            if (axis.name == "axis Year")
            {
                Transform attributeLabel = axis.transform.Find("AttributeLabel");
                attributeLabel.localPosition = new Vector3(-0.1f, 0.5f, 0);
                attributeLabel.localEulerAngles = new Vector3(180, -180, 90);

                axis.transform.Find("AxisLabels").localPosition = new Vector3(0.34f, 0, 0);

                for (int i = 1; i < axis.transform.Find("AxisLabels").childCount; i++)
                {
                    TextMeshPro tmp = axis.transform.Find("AxisLabels").GetChild(i).GetComponent<TextMeshPro>();
                    tmp.text = (1999 + i) + "";
                }

                if (axis.transform.GetSiblingIndex() == 6)
                {
                    attributeLabel = axis.transform.Find("AttributeLabel");
                    attributeLabel.localEulerAngles = new Vector3(180, 0, -90);

                    for (int i = 1; i < axis.transform.Find("AxisLabels").childCount; i++)
                    {
                        axis.transform.Find("AxisLabels").GetChild(i).localEulerAngles = new Vector3(180, 0, 0);
                    }
                }
            }

            if (axis.name == "axis Value")
            {
                if (axis.transform.GetSiblingIndex() == 5) {
                    Transform attributeLabel = axis.transform.Find("AttributeLabel");
                    attributeLabel.localEulerAngles = new Vector3(0, 180, 90);

                    axis.transform.Find("AxisLabels").localEulerAngles = new Vector3(0, 150, 0);
                }
                Transform valueAxisLabels = axis.transform.Find("AxisLabels");

                if (valueAxisLabels.childCount != 6)
                {
                    Debug.LogError("wrong value label child count");
                }
                else {
                    for (int i = 0; i < 5; i++) {
                        valueAxisLabels.GetChild(i + 1).GetComponent<TextMeshPro>().text = (25 * i) + "%";
                    }
                    valueAxisLabels.GetChild(5).GetComponent<TextMeshPro>().text = "100%";
                }
            }
        }

        return v;
    }


    protected GameObject CreateAxis(AbstractVisualisation.PropertyType propertyType, DimensionFilter dimensionFilter, Vector3 position, Vector3 rotation, Vector3 scale, int index, CSVDataSource csvds, Visualisation vis, GameObject go)
    {
        GameObject AxisHolder;

        AxisHolder = (GameObject)Instantiate(Resources.Load("Axis"));

        AxisHolder.transform.parent = go.transform;
        AxisHolder.name = propertyType.ToString();
        AxisHolder.transform.eulerAngles = (rotation);
        AxisHolder.transform.localPosition = position;
        AxisHolder.transform.localScale = scale;

        Axis axis = AxisHolder.GetComponent<Axis>();
        axis.SetDirection((int)propertyType);

        // justify country text alignment
        if (index == 0 || index == 3) {
            axis.Length = 1.1f;
        }

        axis.Init(csvds, dimensionFilter, vis);
        BindMinMaxAxisValues(axis, dimensionFilter, vis);

        AxisHolder.transform.Find("axis_mesh").GetChild(0).gameObject.AddComponent(typeof(CapsuleCollider));

        AxisHolder.transform.Find("axis_mesh").localPosition = new Vector3(0, -0.125f, 0);
        AxisHolder.transform.Find("axis_mesh").localScale = new Vector3(0.05f, 1.25f, 0.05f);

        CapsuleCollider cc = AxisHolder.transform.Find("axis_mesh").GetChild(0).GetComponent<CapsuleCollider>();
        cc.radius = 3f;
        cc.height = 2f;


        if (AxisHolder.name == "axis Value")
        {
            AxisHolder.transform.Find("Cone").localPosition = new Vector3(0, 1, 0);
            AxisHolder.transform.Find("axis_mesh").localPosition = new Vector3(0, 0f, 0);
            AxisHolder.transform.Find("axis_mesh").localScale = new Vector3(0.05f, 1f, 0.05f);
        }

        return AxisHolder;
    }

    protected void BindMinMaxAxisValues(Axis axis, DimensionFilter dim, Visualisation vis)
    {
        object minvalue = vis.dataSource.getOriginalValue(dim.minFilter, dim.Attribute);
        object maxvalue = vis.dataSource.getOriginalValue(dim.maxFilter, dim.Attribute);

        object minScaledvalue = vis.dataSource.getOriginalValue(dim.minScale, dim.Attribute);
        object maxScaledvalue = vis.dataSource.getOriginalValue(dim.maxScale, dim.Attribute);

        axis.AttributeFilter = dim;
        axis.UpdateLabelAttribute(dim.Attribute);

        axis.SetMinNormalizer(dim.minScale);
        axis.SetMaxNormalizer(dim.maxScale);
    }

    //private void UpdateAxisBrushing() {
    //    // check intersection
    //    if (leftYearBrushControl == 0 && leftCountryBrushControl != 0 && rightYearBrushControl != 0 && rightCountryBrushControl == 0)
    //    {
    //        chessBoardBrushingBool = new bool[100];
    //        chessBoardBrushingBool[10 * (leftCountryBrushControl - 1) + (rightYearBrushControl - 1)] = true;
    //        smms.SetFilterPosition(leftCountryBrushControl - 1, leftCountryBrushControl, rightYearBrushControl - 1, rightYearBrushControl);
    //    }
    //    else if (leftYearBrushControl != 0 && leftCountryBrushControl == 0 && rightYearBrushControl == 0 && rightCountryBrushControl != 0)
    //    {
    //        chessBoardBrushingBool = new bool[100];
    //        chessBoardBrushingBool[10 * (rightCountryBrushControl - 1) + (leftYearBrushControl - 1)] = true;
    //        smms.SetFilterPosition(rightCountryBrushControl - 1, rightCountryBrushControl, leftYearBrushControl - 1, leftYearBrushControl);
    //    }
    //    else if (leftYearBrushControl == 0 && leftCountryBrushControl != 0 && rightYearBrushControl == 0 && rightCountryBrushControl != 0) // check parallel
    //    {
    //        if (Math.Abs(leftCountryBrushControl - rightCountryBrushControl) != 0 && Math.Abs(leftCountryBrushControl - rightCountryBrushControl) != 1)
    //        {
    //            for (int i = Math.Min(leftCountryBrushControl, rightCountryBrushControl) + 1; i < Math.Max(leftCountryBrushControl, rightCountryBrushControl); i++)
    //            {
    //                for (int j = 0; j < 10; j++)
    //                {
    //                    chessBoardBrushingBool[(i - 1) * 10 + j] = true;
    //                }
    //            }
    //        }
    //        if (leftCountryBrushControl < rightCountryBrushControl)
    //        {
    //            smms.SetFilterPosition(leftCountryBrushControl - 1, rightCountryBrushControl, 0, 10);
    //        }
    //        else
    //        {
    //            smms.SetFilterPosition(leftCountryBrushControl, rightCountryBrushControl - 1, 0, 10);
    //        }

    //    }
    //    else if (leftYearBrushControl != 0 && leftCountryBrushControl == 0 && rightYearBrushControl != 0 && rightCountryBrushControl == 0) // check parallel
    //    {
    //        if (Math.Abs(leftYearBrushControl - rightYearBrushControl) != 0 && Math.Abs(leftYearBrushControl - rightYearBrushControl) != 1)
    //        {
    //            for (int i = Math.Min(leftYearBrushControl, rightYearBrushControl) + 1; i < Math.Max(leftYearBrushControl, rightYearBrushControl); i++)
    //            {
    //                for (int j = 0; j < 10; j++)
    //                {
    //                    chessBoardBrushingBool[j * 10 + (i - 1)] = true;
    //                }
    //            }
    //        }
    //        if (leftYearBrushControl < rightYearBrushControl)
    //        {
    //            smms.SetFilterPosition(0, 10, leftYearBrushControl - 1, rightYearBrushControl);
    //        }
    //        else
    //        {
    //            smms.SetFilterPosition(0, 10, leftYearBrushControl, rightYearBrushControl - 1);
    //        }
    //    }
    //    else
    //    {

    //        if (rightCountryBrushControl == 0 && leftCountryBrushControl != 0)
    //        {
    //            if (leftYearBrushControl == 0 && rightYearBrushControl == 0)
    //            {
    //                smms.SetFilterPosition(leftCountryBrushControl - 1, leftCountryBrushControl, 0, 10);
    //            }
    //            else
    //            {
    //                smms.SetFilterPosition(leftCountryBrushControl - 1, leftCountryBrushControl, -1, -1);
    //            }
    //        }
    //        else if (leftCountryBrushControl == 0 && rightCountryBrushControl != 0)
    //        {
    //            if (leftYearBrushControl == 0 && rightYearBrushControl == 0)
    //            {
    //                smms.SetFilterPosition(rightCountryBrushControl - 1, rightCountryBrushControl, 0, 10);
    //            }
    //            else
    //            {
    //                smms.SetFilterPosition(rightCountryBrushControl - 1, rightCountryBrushControl, -1, -1);
    //            }
    //        }

    //        if (rightYearBrushControl == 0 && leftYearBrushControl != 0)
    //        {
    //            if (leftCountryBrushControl == 0 && rightCountryBrushControl == 0)
    //            {
    //                smms.SetFilterPosition(0, 10, leftYearBrushControl - 1, leftYearBrushControl);
    //            }
    //            else
    //            {
    //                smms.SetFilterPosition(-1, -1, leftYearBrushControl - 1, leftYearBrushControl);
    //            }
    //        }
    //        else if (rightYearBrushControl != 0 && leftYearBrushControl == 0)
    //        {
    //            if (leftCountryBrushControl == 0 && rightCountryBrushControl == 0)
    //            {
    //                smms.SetFilterPosition(0, 10, rightYearBrushControl - 1, rightYearBrushControl);
    //            }
    //            else
    //            {
    //                smms.SetFilterPosition(-1, -1, rightYearBrushControl - 1, rightYearBrushControl);
    //            }
    //        }

    //    }
    //}

    public void UpdateBrushing(bool[] calculatedChessBoard)
    {
        chessBoardBrushingBool = calculatedChessBoard;

        Color[] dataUpdatedColors = new Color[customCSVData.DataCount];

        float[] barData = customCSVData["Year"].Data;
        Color[] OldMappedTransparentColors = new Color[100];

        for (int i = 0; i < UpdatedMappedColors.Length; i++)
        {
            OldMappedTransparentColors[i] = UpdatedMappedColors[i];
            OldMappedTransparentColors[i].a = 0.3f;
        }

        for (int i = 0; i < barData.Length; i++)
        {
            if (chessBoardBrushingBool[i])
            {
                dataUpdatedColors[i] = OldMappedColors[i];
            }
            else
            {
                dataUpdatedColors[i] = OldMappedTransparentColors[i];
            }
        }

        SetColors(dataUpdatedColors);
    }


    //private void StopBrushing()
    //{
    //    smms.ResetFilters();

    //    foreach (View v in finalVs)
    //    {
    //        v.SetColors(OldMappedColors);
    //    }
    //}

    //public void RemoveHighlightedBars(List<Vector2> removedBars, string barChartName) {
    //    this.removedBars.Add(barChartName, removedBars);
    //}

    public void SetColors(Color[] dataUpdatedColors) {
        foreach (View v in finalVs)
        {
            v.SetColors(dataUpdatedColors);
        }
        UpdatedMappedColors = dataUpdatedColors;
    }

    // Update is called once per frame
    void Update()
    {
    
    }
}
