using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using ExtensionMethods;

[RequireComponent(typeof(Chance))]
public class StarBaseGenerator : MonoBehaviour
{
    public int instances;
    public float MinScale;
    public float MaxScale;

    public StarbaseModuleCatalog catalog;

    private Chance Chance;

    // Use this for initialization
    void Start()
    {
        Chance = GetComponent<Chance>();

        generate();
    }

    public void generate()
    {
        //var rootPrefabs = catalog.prefabs.Where(x => x.GetComponent<StarBaseModule>().ModuleType == StarBaseModule.eModuleType.Root);
      //  Debug.LogFormat("{0} root prefabs", rootPrefabs.Count());
        var rootPrefab =  GameObject.Instantiate(Chance.Pick<GameObject>(catalog.Hubs));

        rootPrefab.transform.localScale = Vector3.one * Chance.RandomFloat(MinScale, MaxScale);

        rootPrefab.transform.position = transform.position;
       // rootPrefab.transform.rotation = Quaternion.Euler(Chance.Vector(360f));
        rootPrefab.transform.parent = transform;
        generateChildren(rootPrefab, instances);
    }

    public void generateChildren(GameObject root, int levels)
    {
        var connectors = root.GetComponentsInChildren<StarBaseConnector>();

        if (connectors == null || connectors.Count() == 0)
            return;

        int lastTypeSet = connectors.Select(x => x.TypeSet).Max();

        GameObject[] typeSetInstances = new GameObject[lastTypeSet + 1];

        foreach (StarBaseConnector rootConnector in connectors)
        {
            if (rootConnector.ConnectedTo != null) continue;
            if (rootConnector.name == "RootConnector") continue;

            GameObject branch = null;
            StarBaseModule.eModuleType childType;
            bool isInstance = false;


            if (typeSetInstances[rootConnector.TypeSet] == null)
            {
                if(levels > 0)
                {
                    float f = Chance.RandomFloat(1f);

                    if (f < rootConnector.RootRatio)
                    {
                        childType = StarBaseModule.eModuleType.Root;
                    }
                    else
                    {
                        childType = StarBaseModule.eModuleType.Branch;
                    }
                } else
                {
                    childType = StarBaseModule.eModuleType.Leaf;
                }

                //var childPrefabs = catalog.prefabs.Where(x => x.GetComponent<StarBaseModule>().ModuleType == childType).ToArray();

                isInstance = false;
                switch (childType)
                {
                    case StarBaseModule.eModuleType.Root:
                        branch = GameObject.Instantiate(Chance.Pick<GameObject>(catalog.Hubs));
                        break;
                    case StarBaseModule.eModuleType.Branch:
                        branch = GameObject.Instantiate(Chance.Pick<GameObject>(catalog.Branches));
                        break;
                    case StarBaseModule.eModuleType.Leaf:
                        branch = GameObject.Instantiate(Chance.Pick<GameObject>(catalog.Leaves));
                        break;
                }
                
                if(branch == null)
                {
                    Debug.LogWarningFormat("{0} can't pick a {1}", this.PathID(), childType);

                    return;
                }

                var module = branch.GetComponent<StarBaseModule>();

                if(module != null)
                {
                    if (module.isScalable)
                    {
                        branch.transform.localScale = Vector3.one * Chance.RandomFloat(rootConnector.MaxScale, rootConnector.MinScale);
                    }
                    else
                    {
                        branch.transform.localScale = Vector3.one;
                    }
                }


                typeSetInstances[rootConnector.TypeSet] = branch;
            }
            else
            {
                isInstance = true;
                branch = GameObject.Instantiate(typeSetInstances[rootConnector.TypeSet]);
                childType = branch.GetComponent<StarBaseModule>().ModuleType;
                branch.transform.localScale = Vector3.one;
            }

            var branchConnector = GetRootConnector(branch);

            if(branchConnector == null)
            {
                Debug.LogWarningFormat("{0} has no connectors!", branch.name);
                continue;
            }

            //reverse the heirarchy of branch and branchConnector, adding them into the heirarchy of root, off of rootConnector
            branchConnector.transform.SetParent(rootConnector.transform);
            branch.transform.parent = branchConnector.transform;

            //move branchConnector (and it's child Branch) to the same postion as it's parent (rootConnector)
            branchConnector.transform.localPosition = Vector3.zero;

            //this is not right...
            //rotate the branchConnector with respect to rootConnector
            branchConnector.transform.rotation = rootConnector.transform.rotation;

            //Check to see of this new branch is going to collide/intersect with anything (if we don't do this then end up like catamari)
            if(childType != StarBaseModule.eModuleType.Leaf)
            {

                var branchColliders = branchConnector.GetComponents<Collider>().ToList();

                branchColliders.AddRange(branchConnector.GetComponents<BoxCollider>());
                branchColliders.AddRange(branchConnector.GetComponents<CapsuleCollider>());
                branchColliders.AddRange(branchConnector.GetComponents<MeshCollider>());
                branchColliders.AddRange(branchConnector.GetComponentsInChildren<BoxCollider>());
                branchColliders.AddRange(branchConnector.GetComponentsInChildren<CapsuleCollider>());
                branchColliders.AddRange(branchConnector.GetComponentsInChildren<MeshCollider>());

                if (branchColliders.Count == 0)
                {
                    Debug.LogWarningFormat("{0} branch has no colliders", branch.PathID());
                }
                else
                {
                    var bounds = branchColliders.Where(c=>c.isTrigger == false).Select(c => c.bounds).ToArray();
                    branchConnector.gameObject.SetActive(false);
                    bool isBlocked = false;

                    foreach (Bounds b in bounds)
                    {
                        if (Physics.CheckBox(b.center, b.extents))
                        {
                            isBlocked = true;

                            Debug.LogWarningFormat("{0} branch blocked", branch.PathID());

                            break;
                        }
                    }

                    if (isBlocked)
                    {
                        GameObject.Destroy(branch);

                        continue;
                    }
                    else
                    {
                        branchConnector.gameObject.SetActive(true);
                    }
                }
            }

            //3) Profit!

            //parent branch to root
            branch.transform.SetParent(rootConnector.transform);
            //parent branch connector back to branch
            branchConnector.transform.SetParent(branch.transform);
           
            //let both connectors know what modules they are connected to
            branchConnector.ConnectedTo = root;
            rootConnector.ConnectedTo = branch;

            //if(branch.GetComponent<StarBaseModule>().IsConflicted)
            //{
            //    Debug.LogFormat("module {0} in {1} is conflicted", branch.name, gameObject.name);
            //}

            if (isInstance || childType == StarBaseModule.eModuleType.Leaf) {
                continue;
            }

            generateChildren(branch, levels - 1);
        } //end foreach
    } //end generateChildren

    private StarBaseConnector GetRootConnector(GameObject branch)
    {
        Transform rct =  branch.transform.Find("RootConnector");

        if(rct == null)
        {
            return branch.GetComponentInChildren<StarBaseConnector>();
        } else
        {
            return rct.GetComponent<StarBaseConnector>();
        }
    }

    public Quaternion Invert (Quaternion subject)
    {
        Vector3 ea = subject.eulerAngles;

        /*if (ea.x != 0f)*/ ea.x += 180f;
        /*if (ea.y != 0f)*/ ea.y += 180f;
        /*if (ea.z != 0f)*/ ea.z += 180f;
        if (ea.x > 360f) ea.x -= 360f;
        if (ea.y > 360f) ea.y -= 360f;
        if (ea.z > 360f) ea.z -= 360f;

        return Quaternion.Euler(ea);
    }
}