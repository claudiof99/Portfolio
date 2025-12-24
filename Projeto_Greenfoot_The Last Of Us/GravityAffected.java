import greenfoot.*;  // (World, Actor, GreenfootImage, Greenfoot and MouseInfo)

/**
 * Write a description of class GravityAffected here.
 * 
 * @author (your name) 
 * @version (a version number or a date)
 */
public class GravityAffected extends Entity
{
    // gravity value "acceleration"
    private int _gravity = 1;
    // vertical velocity that will be incremented with gravity or a jump force (declared in entity class)
    private int _verticalVelocity = 0;
    private boolean _fallingOf = false;
    // private Vector2 prevDir = getDirection();
    /**
     * returns true if object's image is touching a Ground class and false otherwise
     */
    protected boolean isGrounded(Class <?> floor)
    {
        // check if the object is colliding with a ground class
        Actor under = getOneObjectAtOffset(0, getSprite().getHeight()/2 - 20, floor);
        //under will return an object so returns null if there is no ground        
        return under != null;
    }
    
    /**
     * Boolean to check if the player is on ground. 
     * True if on the ground, false otherwise
     */
    protected boolean isGrounded()
    {
        return isGrounded(Platform.class);
    }
    
    /**
     * Sets the falling velocity for the entities that are affected by gravity
     */
    protected void fall()
    {
        // set location incrementaly with vertical velocity
        setLocation(getX(), getY() + _verticalVelocity);
        // prevDir = getDirection();
        if(this instanceof Player)
            setDirection(Vector2.VectorDOWN);
        // add gravity to velocity every frame making acceleration
        _verticalVelocity += _gravity;
    }
    
    /**
     * Not much to say, jump function for the entity
     * @param int verticalForce - force of gravity
     */
    protected void jump(int verticalForce)
    {
        // decrease vertical velocity to simulate a jump
        if(this instanceof Player)
            setDirection(Vector2.VectorUP);
        _verticalVelocity = -verticalForce;
        // call method fall to update gravity
        fall();
    }
    
    /**
     * Overload for the jump function for a default value of the jump being the entity jump force
     */
    protected void jump(){
        jump(this.getJumpForce());
    }
    
    /**
     * Checks the collision on the y axis, for ground and platforms
     */
    public void checkYCollision()
    {
        // if object is not grounded then its falling otherwise it has 0 vertical velocity
        if (!isGrounded() || (_fallingOf))
            fall();
        if (isGrounded(Ground.class))
        {
            _verticalVelocity = 0;
            _fallingOf = false;
        }
        // if the entity is on a platform and is not trying to fall from it then vVelocity is 0 as not to accelerate when falling of the platform
        if (isGrounded() && !_fallingOf)
        {
            _verticalVelocity = 0;
        }
        // if its not on a platform and is falling (given by the Y direction of the vector) then it means it can try to fall of the platform it is in
        // makes it so it does not fall imediatly off the platform bellow
        // Example
        /*   P
         * ---- player is here
         *  P
         * ---- can fall here and wont fall to the ground imediatly
         * 
         * Otherwise
         *  P
         * ---- -> Platform
         * 
         * ---- -> Platform
         *  P
         * ..... -> Ground
         */
        if (!isGrounded() && getDirection().isEqual(Vector2.VectorDOWN))
            _fallingOf = false;
    }
    
    /**
     * Called whenever the player is falling to set the variable true
     */
    public void fallOf()
    {
        this._fallingOf = true;
    }
}
