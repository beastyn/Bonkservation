using AI.Tree.Idols;
using AI.Tree;
using AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleAIBehaviour : MonoBehaviour
{

    [SerializeField] MischieveHelper mischieveHelper;

    BehaviourTree tree;
    Node.Status treeStatus = Node.Status.Running;



    void Start()
    {
        this.tree = new BehaviourTree();

        Sequence mischieve = new Sequence("Mischieve", mischieveHelper.FailHandler);
            Leaf requestMischieveObject = new Leaf("Request mischieve object", mischieveHelper.RequestMischieveObject);
            Leaf goToMischieveObject = new Leaf("Go to mischieve object", mischieveHelper.GoToMischieveObject);
            Leaf stayAtMischieve = new Leaf("Stay at mischieve object", mischieveHelper.StayAtMischieveObject);

        mischieve.AddChild(requestMischieveObject);
        mischieve.AddChild(goToMischieveObject);
        mischieve.AddChild(stayAtMischieve);

        this.tree.AddChild(mischieve);

    }

    void Update()
    {
        this.treeStatus = this.tree.Process();
    }
}
