class Component
{
    protected Actor mActor;
    public Component(Actor actor)
    {
        mActor = actor; 

        mActor.AddComponent(this);
    }

    public void Update(float deltaTIme)
    {

    }
    
}