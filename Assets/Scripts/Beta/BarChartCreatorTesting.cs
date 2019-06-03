using IATK;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class BarChartCreatorTesting : MonoBehaviour {

    // input data
    public List<TextAsset> datasets;

    public List<GameObject> barList;

    public int highlightMethod = 1;

    List<GameObject> axisList;

    CSVDataSource customCSVData;
    List<View> finalVs;
    Color[] OldMappedColors;
    Color[] UpdatedMappedColors;

    [HideInInspector]
    public int yearBrushControl = 0;
    [HideInInspector]
    public int countryBrushControl = 0;

    [HideInInspector]
    public bool brushingNextYear = false;
    [HideInInspector]
    public bool brushingPreviousYear = false;
    [HideInInspector]
    public bool brushingNextCountry = false;
    [HideInInspector]
    public bool brushingPreviousCountry = false;
    // Use this for initialization
    void Awake()
    {
        barList = new List<GameObject>();
        axisList = new List<GameObject>();
        finalVs = new List<View>();
        int count = 0;

        if (GameObject.Find("SmallMultiplesManager") != null) {
            GameObject smm = GameObject.Find("SmallMultiplesManager");
            SmallMultiplesManagerScript smms = smm.GetComponent<SmallMultiplesManagerScript>();

            datasets.Clear();
            for (int i = 1; i <= smms.smallMultiplesNumber; i++)
            {
                TextAsset file = (TextAsset)Resources.Load("bData" + i);
                datasets.Add(file);
            }
        }
 
        foreach (TextAsset ta in datasets)
        {

            GameObject go = new GameObject("BarCharts-" + count);
            go.transform.parent = this.transform;
            go.transform.localScale = new Vector3(0.35f, 0.35f, 0.35f);
            go.transform.localPosition += Vector3.up * 1;

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
            //    return new Color(215f / fc, 48f / fc, 39f / fc); // red
            //case 1:
            //    return new Color(244f / fc, 109f / fc, 67f / fc); // orange 
            //case 2:
            //    return new Color(255f / fc, 200f / fc, 0f / fc); // yellow (255,150,0)
            //case 3:
            //    return new Color(138f / fc, 255f / fc, 0f / fc); //
            //case 4:
            //    return new Color(255f / fc, 255f / fc, 191f / fc); // 
            //case 5:
            //    return new Color(0f / fc, 255f / fc, 16f / fc);
            //case 6:
            //    return new Color(26f / fc, 152f / fc, 80f / fc); // green
            //case 7:
            //    return new Color(0f / fc, 255f / fc, 244f / fc); // sky
            //case 8:
            //    return new Color(0f / fc, 100f / fc, 255f / fc); // blue
            //case 9:
            //    return new Color(84f / fc, 39f / fc, 136f / fc); // purple
            //default:
            //    return Color.white;
            case 0:
                return new Color(215f / fc, 48f / fc, 39f / fc); 
            case 1:
                return new Color(244f / fc, 109f / fc, 67f / fc);
            case 2:
                return new Color(253f / fc, 174f / fc, 97f / fc); 
            case 3:
                return new Color(254f / fc, 224f / fc, 144f / fc);
            case 4:
                return new Color(255f / fc, 255f / fc, 191f / fc);
            case 5:
                return new Color(224f / fc, 243f / fc, 248f / fc); 
            case 6:
                return new Color(171f / fc, 217f / fc, 233f / fc); 
            case 7:
                return new Color(116f / fc, 173f / fc, 209f / fc); 
            case 8:
                return new Color(69f / fc, 117f / fc, 180f / fc); 
            case 9:
                return new Color(49f / fc, 54f / fc, 149f / fc);
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

        Color[] colourPalette = new Color[10];

        for (int i = 0; i < 10; i++) {
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
        mt.SetFloat("_MinSize", 1.7f); //0.01f
        mt.SetFloat("_MaxSize", 1.7f); //0.05f


        View v = vb.updateView().apply(go, mt);

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
        posx.z = -0.1f;
        DimensionFilter xDimension = new DimensionFilter { Attribute = "Country" };
        GameObject X_AXIS = CreateAxis(AbstractVisualisation.PropertyType.X, xDimension, posx, new Vector3(0f, 0f, 0f), globalScale, 0, csvds, visualisation, go);
        axisList.Add(X_AXIS);

        Vector3 posy = Vector3.zero;
        posy.x = -0.1f;
        posy.z = -0.1f;
        DimensionFilter yDimension = new DimensionFilter { Attribute = "Value" };
        GameObject Y_AXIS = CreateAxis(AbstractVisualisation.PropertyType.Y, yDimension, posy, new Vector3(0f, 0f, 0f), globalScale, 1, csvds, visualisation, go);
        axisList.Add(Y_AXIS);

        Vector3 posz = Vector3.zero;
        posz.x = -0.1f;
        posz.y = -0.05f;
        posz.y = -0.05f;
        DimensionFilter zDimension = new DimensionFilter { Attribute = "Year" };
        GameObject Z_AXIS = CreateAxis(AbstractVisualisation.PropertyType.Z, zDimension, posz, new Vector3(0f, 90f, 90f), globalScale, 2, csvds, visualisation, go);
        axisList.Add(Z_AXIS);

        // testing new axises
        posx.y = -0.05f;
        posx.z = 1.1f;
        xDimension = new DimensionFilter { Attribute = "Country" };
        X_AXIS = CreateAxis(AbstractVisualisation.PropertyType.X, xDimension, posx, new Vector3(0f, 0f, 0f), globalScale, 3, csvds, visualisation, go);
        axisList.Add(X_AXIS);

        posy.x = 1.1f;
        posy.z = 1.1f;
        yDimension = new DimensionFilter { Attribute = "Value" };
        Y_AXIS = CreateAxis(AbstractVisualisation.PropertyType.Y, yDimension, posy, new Vector3(0f, 0f, 0f), globalScale, 4, csvds, visualisation, go);
        axisList.Add(Y_AXIS);

        posz.x = 1.1f;
        posz.y = -0.05f;
        zDimension = new DimensionFilter { Attribute = "Year" };
        Z_AXIS = CreateAxis(AbstractVisualisation.PropertyType.Z, zDimension, posz, new Vector3(0f, 90f, 90f), globalScale, 5, csvds, visualisation, go);
        axisList.Add(Z_AXIS);

        // end testing new axises

        foreach (GameObject axis in axisList) {
            axis.transform.Find("MinNormaliser").gameObject.SetActive(false);
            axis.transform.Find("MaxNormaliser").gameObject.SetActive(false);

            axis.transform.Find("Cone").localScale = new Vector3(0.04f, 0.8f, 0.04f);

            Transform axisLabels = axis.transform.Find("AxisLabels");
            foreach (Transform t in axisLabels) {
                TextMeshPro tmp = t.GetComponent<TextMeshPro>();
                tmp.fontSize = 0.5f;
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

                
            }

            if (axis.name == "axis Year")
            {
                Transform attributeLabel = axis.transform.Find("AttributeLabel");
                attributeLabel.localPosition = new Vector3(-0.1f, 0.5f, 0);
                attributeLabel.localEulerAngles = new Vector3(180, -180, 90);

                axis.transform.Find("AxisLabels").localPosition = new Vector3(0.26f, 0, 0);

                for (int i = 1; i < axis.transform.Find("AxisLabels").childCount; i++)
                {
                    TextMeshPro tmp = axis.transform.Find("AxisLabels").GetChild(i).GetComponent<TextMeshPro>();
                    tmp.text = tmp.text.Substring(0, 4);
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

                    axis.transform.Find("AxisLabels").localEulerAngles = new Vector3(0, 180, 0);
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

    //delegate float[] Filter(float[] ar, CSVDataSource csvds, string fiteredValue, string filteringAttribute);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="csvds">CSV data source</param>
    /// <param name="filteringValue"> filtered value</param>
    /// <param name="filteringAttribute"> filtering attribute </param>
    /// <param name="color"></param>
    /// <returns></returns>

    public void increaseYearBrushIndex() {
        if (yearBrushControl != 10)
        {
            yearBrushControl++;
        }
        else {
            yearBrushControl = 0;
        }
        brushingNextYear = true;
    }

    public void UnclickTop() {
        brushingNextYear = false;
    }

    public void decreaseYearBrushIndex()
    {
        if (yearBrushControl != 0)
        {
            yearBrushControl--;
        }
        else
        {
            yearBrushControl = 10;
        }
        brushingPreviousYear = true;
    }

    public void UnclickBottom()
    {
        brushingPreviousYear = false;
    }

    public void increaseCountryBrushIndex()
    {
        if (countryBrushControl != 10)
        {
            countryBrushControl++;
        }
        else
        {
            countryBrushControl = 0;
        }
        brushingPreviousCountry = true;
    }

    public void UnclickLeft()
    {
        brushingPreviousCountry = false;
    }

    public void decreaseCountryBrushIndex()
    {
        if (countryBrushControl != 0)
        {
            countryBrushControl--;
        }
        else
        {
            countryBrushControl = 10;
        }
        brushingNextCountry = true;
    }

    public void UnclickRight()
    {
        brushingNextCountry = false;
    }

    public void SetBrushingIndex(int countryIndex, int yearIndex) {
        yearBrushControl = yearIndex;
        countryBrushControl = countryIndex;
    }


    // Update is called once per frame
    void Update()
    {
        // brushing
        Color[] yearUpdatedColors = new Color[customCSVData.DataCount];
        Color[] countryUpdatedColors = new Color[customCSVData.DataCount];
        Color[] bothUpdatedColors = new Color[customCSVData.DataCount];

        if (yearBrushControl != 0 && countryBrushControl == 0)
        {
            float[] yearData = customCSVData["Year"].Data;
            Color[] OldMappedTransparentColors = new Color[customCSVData.DataCount];


            for (int l = 0; l < UpdatedMappedColors.Length; l++)
            {
                OldMappedTransparentColors[l] = UpdatedMappedColors[l];
                OldMappedTransparentColors[l].a = 0.3f;
            }

            for (int i = 0; i < yearData.Length; i++)
            {
                if (i % 10 == (yearBrushControl - 1))
                {
                    if (highlightMethod == 1)
                    {
                        float h;
                        float s;
                        float v;
                        Color.RGBToHSV(UpdatedMappedColors[i], out h, out s, out v);
                        Color highSaturatedColor = Color.HSVToRGB(h, 1, v);
                        yearUpdatedColors[i] = highSaturatedColor;
                    }
                    else if (highlightMethod == 2)
                    {
                        yearUpdatedColors[i] = OldMappedColors[i];
                    }
                    else
                    {
                    }
                }
                else
                {
                    if (highlightMethod == 1)
                    {
                        float h;
                        float s;
                        float v;
                        Color.RGBToHSV(UpdatedMappedColors[i], out h, out s, out v);
                        Color lowSaturatedColor = Color.HSVToRGB(h, 0.2f, v);
                        yearUpdatedColors[i] = lowSaturatedColor;
                    }
                    else if (highlightMethod == 2)
                    {
                        yearUpdatedColors[i] = OldMappedTransparentColors[i];
                    }
                    else
                    {
                    }
                }
            }

            foreach (View v in finalVs)
            {
                v.SetColors(yearUpdatedColors);
            }
            UpdatedMappedColors = yearUpdatedColors;
        }
        else if (countryBrushControl != 0 && yearBrushControl == 0)
        {
            float[] yearData = customCSVData["Country"].Data;
            Color[] OldMappedTransparentColors = new Color[customCSVData.DataCount];

            for (int l = 0; l < UpdatedMappedColors.Length; l++)
            {
                OldMappedTransparentColors[l] = UpdatedMappedColors[l];
                OldMappedTransparentColors[l].a = 0.3f;
            }

            for (int i = 0; i < yearData.Length; i++)
            {
                if (i / 10 * 1.0f == (countryBrushControl - 1))
                {
                    if (highlightMethod == 1)
                    {
                        float h;
                        float s;
                        float v;
                        Color.RGBToHSV(UpdatedMappedColors[i], out h, out s, out v);
                        Color highSaturatedColor = Color.HSVToRGB(h, 1, v);
                        countryUpdatedColors[i] = highSaturatedColor;
                    }
                    else if (highlightMethod == 2)
                    {
                        countryUpdatedColors[i] = OldMappedColors[i];
                    }
                    else
                    {
                    }
                }
                else
                {
                    if (highlightMethod == 1)
                    {
                        float h;
                        float s;
                        float v;
                        Color.RGBToHSV(UpdatedMappedColors[i], out h, out s, out v);
                        Color lowSaturatedColor = Color.HSVToRGB(h, 0.2f, v);
                        countryUpdatedColors[i] = lowSaturatedColor;
                    }
                    else if (highlightMethod == 2)
                    {
                        countryUpdatedColors[i] = OldMappedTransparentColors[i];
                    }
                    else
                    {
                    }
                }
            }

            foreach (View v in finalVs)
            {
                v.SetColors(countryUpdatedColors);
            }
            UpdatedMappedColors = countryUpdatedColors;
        }
        else if (countryBrushControl == 0 && yearBrushControl == 0)
        {
            foreach (View v in finalVs)
            {
                v.SetColors(OldMappedColors);
            }
        }
        else
        {
            float[] data = customCSVData["Country"].Data;
            Color[] OldMappedTransparentColors = new Color[customCSVData.DataCount];

            for (int l = 0; l < UpdatedMappedColors.Length; l++)
            {
                OldMappedTransparentColors[l] = UpdatedMappedColors[l];
                OldMappedTransparentColors[l].a = 0.3f;
            }
            for (int i = 0; i < data.Length; i++)
            {
                if (i / 10 * 1.0f == (countryBrushControl - 1))
                {
                    if (i % 10 == (yearBrushControl - 1))
                    {
                        if (highlightMethod == 1)
                        {
                            float h;
                            float s;
                            float v;
                            Color.RGBToHSV(UpdatedMappedColors[i], out h, out s, out v);
                            Color highSaturatedColor = Color.HSVToRGB(h, 1, v);
                            bothUpdatedColors[i] = highSaturatedColor;
                        }
                        else if (highlightMethod == 2)
                        {
                            bothUpdatedColors[i] = OldMappedColors[i];
                        }
                        else
                        {
                        }
                    }
                    else
                    {
                        if (highlightMethod == 1)
                        {
                            float h;
                            float s;
                            float v;
                            Color.RGBToHSV(UpdatedMappedColors[i], out h, out s, out v);
                            Color lowSaturatedColor = Color.HSVToRGB(h, 0.2f, v);
                            bothUpdatedColors[i] = lowSaturatedColor;
                        }
                        else if (highlightMethod == 2)
                        {
                            bothUpdatedColors[i] = OldMappedTransparentColors[i];
                        }
                        else
                        {
                        }
                    }
                }
                else
                {
                    if (highlightMethod == 1)
                    {
                        float h;
                        float s;
                        float v;
                        Color.RGBToHSV(UpdatedMappedColors[i], out h, out s, out v);
                        Color lowSaturatedColor = Color.HSVToRGB(h, 0.2f, v);
                        bothUpdatedColors[i] = lowSaturatedColor;
                    }
                    else if (highlightMethod == 2)
                    {
                        bothUpdatedColors[i] = OldMappedTransparentColors[i];
                    }
                    else
                    {
                    }
                }
            }
            foreach (View v in finalVs)
            {
                v.SetColors(bothUpdatedColors);
            }
            UpdatedMappedColors = bothUpdatedColors;
        }
        
    }
}
