namespace BrainDesigner.Scripts.Runtime
{
    /// <summary> The final state of a behavior that executes a method and bring visible result.</summary>
    public abstract class Action : Node
    {
        protected abstract bool NeedSceneRefs { get; }
        protected bool ReferenceMissing
        {
            get { return this.referenceMissing; }
            set { this.referenceMissing = value; }
        }

        bool referenceMissing = false;

        protected override void OnAwake()
        {
            base.OnAwake();

            if (this.NeedSceneRefs)
            {
                this.referenceMissing = this.SceneRefs == null && this.AgentObject== null;
            }
        }
    }
}
