import greenfoot.*;  // (World, Actor, GreenfootImage, Greenfoot and MouseInfo)

/**
 * Write a description of class Entity here.
 * 
 * @author Joao Oliveira
 */
public class Entity extends Actor
{
    // how fast the entity will move
    // has a setter and getter
    private int _speed;
    // how much _health will the entity have
    // has a setter and getter
    private int _health;
    // used to verify if the entity is alive or not
    // has a setter and getter
    private boolean _isAlive;
    // jump force for simulating jump
    private int _jumpForce;
    // "skin" of an entity
    // has a setter and getter
    private GreenfootImage _sprite;
    // animation object takes care of animating anything
    // protected due to being used by composition
    private Animation _animation;
    // invicibility timer
    private int _invicibilityTimer;
    // has a setter and getter
    private int _invicibilityCounter;
    // boolean to verify if damage was taken
    // has a setter and getter
    private boolean _tookDamage;
    // direction that the entity is moving
    // deffault to 1 outside constructor because this is an abstract class\
    // has a setter and getter
    private Vector2 _dir;
    // has a setter and getter
    private HitBox _hitbox;
    private Vector2 _hitboxPos;
    // variable for debug ONLY
    private final boolean _DEBUG = false;
    
    /**
     * Constructor for all entities, instantiates animation for each entity, their direction and invincibility frames
     */
    public Entity()
    {
        _animation = new Animation();
        
        _dir = Vector2.VectorRIGHT;
        
        _invicibilityCounter = 0;
        _invicibilityTimer = 50;
    }
    
    /**
     * Method that reduces the _health of the entity
     * 
     * @param damage how much damage will the entity take
     */
    public void takeDamage(int damage)
    {
        // if _health is already 0 no need to calculate damage again so returns
        if(this._health <= 0) return;
        // sets _health
        setHealth(this._health - damage);
        Sound.SelectSound(Sound.PlaySound.GET_HIT, false);
        this._tookDamage = true;
        // start timer of iFrames
        startInvicibilityTimer();
    }
    
    /** Entity specific methods */
    
    /**
     * Starts the invincibility timer for this entity
     */
    private void startInvicibilityTimer()
    {
        this._invicibilityCounter = this._invicibilityTimer;
    }

    /**
     * Spawns hit box for this entity
     */
    protected void spawnHitBox()
    {
        _hitbox.drawHitBox();
        getWorld().addObject(_hitbox,getX(),getY());
    }
    
    /**
     * Constantly updates the hit box location as the entity is walking around the world
     */
    protected void updateHitBoxLocation()
    {
        this._hitboxPos.setVectorX(getX());
        this._hitboxPos.setVectorY(getY() + 35);
        this._hitbox.setLocation(this._hitboxPos.getVectorX(),this._hitboxPos.getVectorY());
    }
    
    /**
     * If _invincibilityCounter is equal to zero then it will set itself to zero, else will reduce the invincibility counter by -1
     */
    protected void reduceInvTimer()
    {
        this._invicibilityCounter = this._invicibilityCounter == 0 ? 
                                    0 : this._invicibilityCounter - 1;
    }
    
    /** Getters and Setters */
    
    /**
     * Returns the entity's animation
     */
    public Animation getAnimation()
    {
        return this._animation;
    }
    
    /**
     * Returns the entity's invincibility counter
     */
    public int getInvicibilitycounter()
    {
        return this._invicibilityCounter;
    }
    
    /**
     * Returns the entity's taken damage
     */
    public boolean getTookDamage()
    {
        return this._tookDamage;
    }
    
    /**
     * Sets the damage taken based on the boolean passed
     */
    public void setTookDamage(boolean b)
    {
        this._tookDamage = b;
    }
    
    /**
     * Setter for _hitbox
     * 
     * @param boolean isPlayer
     */
    protected void makeHitbox(boolean isPlayer)
    {
        _hitbox = new HitBox(isPlayer,_DEBUG,this);
        _hitboxPos = new Vector2();
    }
    
    /**
     * getter for the entity hitbox
     * 
     * @return HitBox _hitbox from entity
     */
    protected HitBox getHitbox()
    {
        return this._hitbox;
    }
    
    /**
     * Setter for _health
     * 
     * @param newHealth new _health of the enity
     */
    public void setHealth(int newHealth)
    {
        this._health = newHealth;
    }

    /**
     * Getter for _health
     * 
     * @return int _health
     */
    public int getHealth()
    {
        return this._health;
    }
    
    /**
     * Getter for the _sprite of the entity
     * 
     * @return GreenfootImage _sprite
     */
    public GreenfootImage getSprite()
    {
        return this._sprite;
    }
    
    /**
     * Setter for the _sprite
     * 
     * @param GreenfootImage _sprite
     */
    public void setSprite(GreenfootImage _sprite)
    {
        this._sprite = _sprite;
    }
    
    /**
     * Setter for the variable _isAlive
     * 
     * @param boolean _isAlive
     */
    public void setIsAlive(boolean _isAlive)
    {
        if(!_isAlive) _hitbox.disableHitBox();
        this._isAlive = _isAlive;
    }
    
    /**
     * Getter for _isAlive
     * 
     * @return boolean _isAlive
     */
    public boolean isAlive()
    {
        return this._isAlive;
    }
    
    public void setDirection(Vector2 dir)
    {
        this._dir = dir;
    }
    
    public Vector2 getDirection()
    {
        return this._dir;
    }
    
    /**
     * Setter for _speed
     * 
     * @param newSpeed new _speed of the entity
     */
    public void setSpeed(int newSpeed)
    {
        this._speed = newSpeed;
    }
    
    /**
     * Getter for _speed
     * 
     * @return _speed
     */
    public int getSpeed()
    {
        return this._speed;
    }
    
    /**
     * Setter for _jumpForce
     * 
     * @param int jf
     */
    public void setJumpForce(int jf)
    {
        this._jumpForce = jf;
    }
    /**
     * Getter for jumforce
     * 
     * @return int jumpforce
     */
    public int getJumpForce()
    {
        return this._jumpForce;
    }
}
