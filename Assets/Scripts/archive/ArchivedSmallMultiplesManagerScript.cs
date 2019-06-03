using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArchivedSmallMultiplesManagerScript : MonoBehaviour {

    //// create building prefabs
    //void CreateBuildingPrefabs()
    //{
    //    if (smallMultiplesNumber < 1)
    //    {
    //        Debug.Log("Please enter a valid small multiples number");
    //    }
    //    else if (smallMultiplesNumber > 12)
    //    {
    //        Debug.Log("More than 12 small multiples are not allowed in this simulation.");
    //    }
    //    else if (smallMultiplesNumber < 4)
    //    {
    //        shelfRows = 1;
    //        shelfItemPerRow = smallMultiplesNumber;
    //        shelfRowY.Add(0);
    //        for (int i = 0; i < smallMultiplesNumber; i++)
    //        {
    //            float xC = 0.3f - 0.3f * smallMultiplesNumber + 0.6f * i;
    //            GameObject buildingObj = (GameObject)Instantiate(BuildingPrefab, new Vector3(xC, 0, 0), Quaternion.identity);
    //            buildingObj.name = (i + 1) + "";
    //            buildingObj.transform.SetParent(this.transform);
    //            buildingObj.transform.localPosition = new Vector3(xC, 0, 0);
    //            buildingSM.Add(buildingObj);
    //        }

    //    }
    //    else if (smallMultiplesNumber == 4)
    //    {
    //        shelfRows = 2;
    //        shelfItemPerRow = 2;
    //        for (int j = 0; j < 2; j++)
    //        {
    //            float yC = 0.15f - 0.3f * j;
    //            shelfRowY.Add(yC);
    //            for (int i = 0; i < 2; i++)
    //            {
    //                float xC = -0.3f + 0.6f * i;
    //                GameObject buildingObj = (GameObject)Instantiate(BuildingPrefab, new Vector3(xC, yC, 0), Quaternion.identity);
    //                buildingObj.name = 2 * j + i + 1 + "";
    //                buildingObj.transform.SetParent(this.transform);
    //                buildingObj.transform.localPosition = new Vector3(xC, yC, 0);
    //                buildingSM.Add(buildingObj);
    //            }
    //        }

    //    }
    //    else
    //    {
    //        int numRows = 1;
    //        int count = 0;
    //        if (smallMultiplesNumber % 4 == 0)
    //        {
    //            numRows = smallMultiplesNumber / 4;
    //        }
    //        else
    //        {
    //            numRows = smallMultiplesNumber / 4 + 1;
    //        }

    //        shelfRows = numRows;
    //        shelfItemPerRow = 4;
    //        for (int j = 0; j < numRows; j++)
    //        {
    //            float yC = -0.15f + 0.15f * numRows - 0.3f * j;
    //            shelfRowY.Add(yC);
    //            for (int i = 0; i < 4; i++)
    //            {
    //                float xC = -0.9f + 0.6f * i;
    //                GameObject buildingObj = (GameObject)Instantiate(BuildingPrefab, new Vector3(xC, yC, 0), Quaternion.identity);
    //                buildingObj.name = 4 * j + i + 1 + "";
    //                buildingObj.transform.SetParent(this.transform);
    //                buildingObj.transform.localPosition = new Vector3(xC, yC, 0);
    //                buildingSM.Add(buildingObj);



    //                count++;
    //                if (count >= smallMultiplesNumber)
    //                {
    //                    break;
    //                }
    //            }
    //        }
    //    }
    //}

    //void ChangeLayout(string layout)
    //{
    //    if (buildingSM != null)
    //    {
    //        GameObject viveCamera = GameObject.Find("Camera (eye)");
    //        // reset rotation
    //        this.transform.rotation = Quaternion.identity;
    //        foreach (GameObject go in buildingSM)
    //        {
    //            go.transform.rotation = Quaternion.identity;
    //        }
    //        if (layout.Equals("2Dmatrix"))
    //        {
    //            if (smallMultiplesNumber < 4)
    //            {
    //                for (int i = 0; i < smallMultiplesNumber; i++)
    //                {
    //                    float xC = 0.3f - 0.3f * smallMultiplesNumber + 0.6f * i;
    //                    GameObject buildingObj = buildingSM[i];
    //                    buildingObj.transform.localPosition = new Vector3(xC, 0, 0);
    //                }
    //            }
    //            else if (smallMultiplesNumber == 4)
    //            {
    //                for (int j = 0; j < 2; j++)
    //                {
    //                    float yC = 0.15f - 0.3f * j;
    //                    for (int i = 0; i < 2; i++)
    //                    {
    //                        float xC = -0.3f + 0.6f * i;
    //                        GameObject buildingObj = buildingSM[2 * j + i];
    //                        buildingObj.transform.localPosition = new Vector3(xC, yC, 0);
    //                    }
    //                }
    //            }
    //            else
    //            {
    //                int numRows = 1;
    //                int count = 0;
    //                if (smallMultiplesNumber % 4 == 0)
    //                {
    //                    numRows = smallMultiplesNumber / 4;
    //                }
    //                else
    //                {
    //                    numRows = smallMultiplesNumber / 4 + 1;
    //                }
    //                for (int j = 0; j < numRows; j++)
    //                {
    //                    float yC = -0.15f + 0.15f * numRows - 0.3f * j;
    //                    for (int i = 0; i < 4; i++)
    //                    {
    //                        float xC = -0.9f + 0.6f * i;
    //                        GameObject buildingObj = buildingSM[4 * j + i];
    //                        buildingObj.transform.localPosition = new Vector3(xC, yC, 0);
    //                        count++;
    //                        if (count >= smallMultiplesNumber)
    //                        {
    //                            break;
    //                        }
    //                    }
    //                }
    //            }
    //        }
    //        else if (layout.Equals("2Dlinear"))
    //        {
    //            for (int i = 0; i < smallMultiplesNumber; i++)
    //            {
    //                float xC = 0.3f - 0.3f * smallMultiplesNumber + 0.6f * i;
    //                GameObject buildingObj = buildingSM[i];
    //                buildingObj.transform.localPosition = new Vector3(xC, 0, 0);
    //            }
    //        }
    //        else if (layout.Equals("2DCmatrix"))
    //        {
    //            if (smallMultiplesNumber < 4)
    //            {
    //                for (int i = 0; i < smallMultiplesNumber; i++)
    //                {
    //                    float xC = 0.3f - 0.3f * smallMultiplesNumber + 0.6f * i;
    //                    GameObject buildingObj = buildingSM[i];
    //                    if (smallMultiplesNumber == 3)
    //                    {
    //                        if (i == 1)
    //                        {
    //                            buildingObj.transform.localPosition = new Vector3(xC, 0, 0.17f);
    //                        }
    //                    }
    //                    else
    //                    {
    //                        buildingObj.transform.localPosition = new Vector3(xC, 0, 0);
    //                    }
    //                }
    //            }
    //            else if (smallMultiplesNumber == 4)
    //            {
    //                for (int j = 0; j < 2; j++)
    //                {
    //                    float yC = 0.15f - 0.3f * j;
    //                    for (int i = 0; i < 2; i++)
    //                    {
    //                        float xC = -0.3f + 0.6f * i;
    //                        GameObject buildingObj = buildingSM[2 * j + i];
    //                        buildingObj.transform.localPosition = new Vector3(xC, yC, 0);
    //                    }
    //                }
    //            }
    //            else
    //            {
    //                int numRows = 1;
    //                int count = 0;
    //                if (smallMultiplesNumber % 4 == 0)
    //                {
    //                    numRows = smallMultiplesNumber / 4;
    //                }
    //                else
    //                {
    //                    numRows = smallMultiplesNumber / 4 + 1;
    //                }
    //                for (int j = 0; j < numRows; j++)
    //                {
    //                    float yC = -0.15f + 0.15f * numRows - 0.3f * j;
    //                    for (int i = 0; i < 4; i++)
    //                    {
    //                        float xC = -0.9f + 0.6f * i;
    //                        GameObject buildingObj = buildingSM[4 * j + i];
    //                        if ((i % 4 == 1) || (i % 4 == 2))
    //                        {
    //                            buildingObj.transform.localPosition = new Vector3(xC, yC, 0.3f);
    //                        }
    //                        else
    //                        {
    //                            buildingObj.transform.localPosition = new Vector3(xC, yC, 0);
    //                        }
    //                        count++;
    //                        if (count >= smallMultiplesNumber)
    //                        {
    //                            break;
    //                        }
    //                    }
    //                }
    //            }
    //        }
    //        else if (layout.Equals("2DClinear"))
    //        {
    //            float xC1 = 0.0f;
    //            float xC2 = 0.0f;
    //            GameObject buildingObj;

    //            if (smallMultiplesNumber % 2 == 0)
    //            {
    //                if (smallMultiplesNumber != 2)
    //                {
    //                    xC1 = 0.3f - 0.3f * smallMultiplesNumber + 0.6f * (smallMultiplesNumber / 2 - 1);
    //                    xC2 = 0.3f - 0.3f * smallMultiplesNumber + 0.6f * (smallMultiplesNumber / 2);
    //                    Vector3 pos1 = new Vector3(xC1, 0, 0);
    //                    buildingSM[smallMultiplesNumber / 2 - 1].transform.localPosition = pos1;
    //                    Vector3 pos2 = new Vector3(xC2, 0, 0);
    //                    buildingSM[smallMultiplesNumber / 2].transform.localPosition = pos2;
    //                    for (int i = smallMultiplesNumber / 2 - 2; i >= 0; i--)
    //                    {
    //                        buildingObj = buildingSM[i];
    //                        buildingObj.transform.localPosition = new Vector3(pos1.x - linearCurvedOffsetx * (smallMultiplesNumber / 2 - 1 - i), pos1.y, pos1.z - linearCurvedOffsetz * (smallMultiplesNumber / 2 - 1 - i));
    //                    }
    //                    for (int i = smallMultiplesNumber / 2 + 1; i < smallMultiplesNumber; i++)
    //                    {
    //                        buildingObj = buildingSM[i];
    //                        buildingObj.transform.localPosition = new Vector3(pos2.x + linearCurvedOffsetx * (i - smallMultiplesNumber / 2), pos2.y, pos2.z - linearCurvedOffsetz * (i - smallMultiplesNumber / 2));
    //                    }
    //                }
    //            }
    //            else
    //            {
    //                if (smallMultiplesNumber != 1)
    //                {
    //                    xC1 = 0.3f - 0.3f * smallMultiplesNumber + 0.6f * (smallMultiplesNumber / 2);
    //                    Vector3 pos1 = new Vector3(xC1, 0, 0);
    //                    buildingSM[smallMultiplesNumber / 2].transform.localPosition = pos1;
    //                    for (int i = smallMultiplesNumber / 2 - 1; i >= 0; i--)
    //                    {
    //                        buildingObj = buildingSM[i];
    //                        buildingObj.transform.localPosition = new Vector3(pos1.x - linearCurvedOffsetx * (smallMultiplesNumber / 2 - i), pos1.y, pos1.z - linearCurvedOffsetz * (smallMultiplesNumber / 2 - i));
    //                    }
    //                    for (int i = smallMultiplesNumber / 2 + 1; i < smallMultiplesNumber; i++)
    //                    {
    //                        buildingObj = buildingSM[i];
    //                        buildingObj.transform.localPosition = new Vector3(pos1.x + linearCurvedOffsetx * (i - smallMultiplesNumber / 2), pos1.y, pos1.z - linearCurvedOffsetz * (i - smallMultiplesNumber / 2));
    //                    }
    //                }
    //            }
    //        }

    //        this.transform.position = viveCamera.transform.position + (viveCamera.transform.forward * 1f) - (viveCamera.transform.up * 0.2f);
    //        Vector3 camPos = viveCamera.transform.position;
    //        Vector3 finalPos = new Vector3(camPos.x, this.transform.position.y, camPos.z);
    //        Vector3 offset = this.transform.position - finalPos;
    //        this.transform.LookAt(this.transform.position + offset);
    //    }
    //    else
    //    {
    //        Debug.Log("Building list is empty!");
    //    }
    //}

    //public void Select2Dmatrix()
    //{
    //    if (!layout.Equals("2Dmatrix"))
    //    {
    //        this.ChangeLayout("2Dmatrix");
    //        layout = "2Dmatrix";
    //    }
    //}

    //public void Select2Dlinear()
    //{
    //    if (!layout.Equals("2Dlinear"))
    //    {
    //        this.ChangeLayout("2Dlinear");
    //        layout = "2Dlinear";
    //    }
    //}

    //public void Select2DCmatrix()
    //{
    //    if (!layout.Equals("2DCmatrix"))
    //    {
    //        this.ChangeLayout("2DCmatrix");
    //        layout = "2DCmatrix";
    //    }
    //}

    //public void Select2DClinear()
    //{
    //    if (!layout.Equals("2DClinear"))
    //    {
    //        this.ChangeLayout("2DClinear");
    //        layout = "2DClinear";
    //    }
    //}

}
