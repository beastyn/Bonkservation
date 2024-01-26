namespace AI.Tree.Idols
{
    using Managers;
    using System.Collections;
    using System.Collections.Generic;
    using System.Xml;
    using UnityEngine;
    using UnityEngine.AI;

    public class IdolBehaviour : MonoBehaviour
    {
        [SerializeField] MischieveHelper mischieveHelper;
        [SerializeField] PawAvoidHelper pawAvoidHelper;
        [SerializeField] RestingHelper restingHelper;
        [SerializeField] IdolInfoSO idolInfo;

        BehaviourTree tree;
        Node.Status treeStatus = Node.Status.Running;



        void Start()
        {
            this.tree = new BehaviourTree();

            Selector restOrActive = new Selector("Rest or active");
                Sequence energyRest = new Sequence("Energy rest");
                    Leaf energyCheck = new Leaf("Energy check", restingHelper.CheckIfNeedRest);
                    Leaf resting = new Leaf("Resting" , restingHelper.Rest);

                Selector avoidOrMischieve = new Selector("Avoid Or Mischieve");
                    Sequence avoidBonk = new Sequence("Avoid Bonk");
                        Leaf checkPaw = new Leaf("Check Paw", this.pawAvoidHelper.CheckPaw);
                        Selector runFromBonk = new Selector("Run From Paw");
                            Sequence wantUseSkill = new Sequence("Want use Skill");
                                Leaf checkSkill = new Leaf("Check Skill", this.pawAvoidHelper.CheckUseSkill);
                                Leaf useSkill = new Leaf("Use skill", this.pawAvoidHelper.UseSkill);
                            Sequence wantDodge = new Sequence("Want dodge");
                                Leaf checkDodge = new Leaf("Check Dodge", this.pawAvoidHelper.CheckNeedDodge);
                                Leaf dodge = new Leaf("Dodge", this.pawAvoidHelper.DodgePaw);
                            Leaf run = new Leaf("Run", this.pawAvoidHelper.RunFromPaw);
                    Sequence mischieve = new Sequence("Mischieve", mischieveHelper.FailHandler);
                        Leaf requestMischieveObject = new Leaf("Request mischieve object", mischieveHelper.RequestMischieveObject);
                        Leaf goToMischieveObject = new Leaf("Go to mischieve object", mischieveHelper.GoToMischieveObject);
                        Leaf stayAtMischieve = new Leaf("Stay at mischieve object", mischieveHelper.StayAtMischieveObject);


            wantDodge.AddChild(checkDodge);
            wantDodge.AddChild(dodge);

            wantUseSkill.AddChild(checkSkill);
            wantUseSkill.AddChild(useSkill);

            runFromBonk.AddChild(wantDodge);
            runFromBonk.AddChild(wantUseSkill);
            runFromBonk.AddChild(run);

            avoidBonk.AddChild(checkPaw);
            avoidBonk.AddChild(runFromBonk);

            mischieve.AddChild(requestMischieveObject);
            mischieve.AddChild(goToMischieveObject);
            mischieve.AddChild(stayAtMischieve);

            energyRest.AddChild(energyCheck);
            energyRest.AddChild(resting);

            avoidOrMischieve.AddChild(avoidBonk);
            avoidOrMischieve.AddChild(mischieve);

            restOrActive.AddChild(energyRest);
            restOrActive.AddChild(avoidOrMischieve);

            this.tree.AddChild(restOrActive);

            //this.tree.PrintTree();

        }

        void Update()
        {
            this.treeStatus = this.tree.Process();
        }
    }
}
