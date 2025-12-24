import greenfoot.*;  // (World, Actor, GreenfootImage, Greenfoot and MouseInfo)
import java.util.*;

/**
 * Write a description of class Player here.
 * 
 * @author Joao Oliveira 
 * @version (a version number or a date)
 */
public class Player extends GravityAffected
{
    // variable used to change controlls
    private boolean _secondPlayer;
    // movement keys
    private String _keyUp, _keyLeft, _keyDown, _keyRight, _actionKey;
    private int _stairSpeed;
    // animation
    //private String _currentAnimationFrame;
    private String _framesPath;
    private String _character;
    // different types of animation
    private final int _NFRAMES;
    private GreenfootImage[] _runAnim;
    private GreenfootImage[] _deathAnim;
    private GreenfootImage[] _idleAnim;
    
    private boolean _hasKey;
    
    private boolean _turn;
    
    private Gun _gun;
    private Vector2 _gunPos;
    private boolean _gunInWorld;
    
    /**
     * Constructor for player
     * 
     * Takes parameters of which keys for what player assuming it will be played on
     * the same keyboard, and a boolean if it is a second player. <- probably not needed
     * 
     * @param  boolean isSecondPlayer - is the second player or not, true  or false
     * @param String keyUP - key to be used to move upwards
     * @param String keyLeft - key to be used to move left
     * @param String keyDown - key to be used to move downwards
     * @param String keyRight - key to be used to move right
     * @param String actionKey - key to interact in this case is used to shoot
     */
    public Player(boolean isSecondPlayer,//
                 String keyUp,
                 String keyLeft, //
                 String keyDown, 
                 String keyRight, 
                 String actionKey
                )
    {         
        setSpeed(5);
     
        setHealth(5);
        
        setIsAlive(true);
        
        setJumpForce(16);
        
        _stairSpeed = 3;
        _secondPlayer = isSecondPlayer;
        
        // Animation
        
        _NFRAMES = 6;
        _turn = false;
        
        if (_secondPlayer)
            this._character = "Char 4";
        else
            this._character = "Char 3";
        
        this._framesPath = "./Full body animated characters/" + _character + "/no hands/";
        
        this._hasKey = false;
        
        _gun = new Gun();
        _gunPos = new Vector2();
        _gunInWorld = true;
        
        makeHitbox(true);
        
        _idleAnim = initAnimation("idle", _NFRAMES); // initiate idle animation has 6 frames
        _runAnim = initAnimation("walk", _NFRAMES + 1); // initiate walk animation has 7 frames
        _deathAnim = initAnimation("death", _NFRAMES + 2); // initiate death animation has 8 frames
        
        getAnimation().setSpriteArray(_idleAnim);
        
        setSprite(_idleAnim[0]);
        
        setTookDamage(false);
        
        this._keyRight = keyRight;
        this._keyLeft = keyLeft;
        this._keyUp = keyUp;
        this._keyDown = keyDown;
        this._actionKey = actionKey;        
        
    }
    
    /**
     * Act function for all players. Checks for inputs, animates the player, checks the collision on the y axis, updates his gun location and checks if
     * fell off the map
     */
    public void act()
    {
        inputListener();
        
        getAnimation().animate(this);
        
        checkYCollision();
        
        updateGunLocation();
        updateHitBoxLocation();
        
        collisionChecker();
        
        fellOffMap();
    }
    
    /**
     * Boolean to check if this player is the first or second
     */
    public boolean isSecond()
    {
        return _secondPlayer;
    }
    
    /**
     * Checks the inputs pressed by the player and calls the respective function for each input
     */
    private void inputListener()
    {
        // if its not alive no need to check for input so returns
        if(isAlive())
        {
            if (Greenfoot.isKeyDown(this._keyLeft))
            {
                // change direction to the left
                setDirection(Vector2.VectorLEFT);
                _gun.setFacingVector(getDirection());
                // verify if it did not turn
                if(!_turn)
                {
                    // flip animation arrays
                    flipSpriteArrays();
                    // set true turn so it does not flip constantly
                    _turn = true;
                }
                // call move to move the player entity
                move();   
            }
            // else if so it cannot move left and right at the same time
            else if (Greenfoot.isKeyDown(this._keyRight))
            {
                // change direction to right
                setDirection(Vector2.VectorRIGHT);
                _gun.setFacingVector(getDirection());
                if(_turn)
                {
                    // flip animation arrays
                    flipSpriteArrays();
                    _turn = false;
                }
                move();
            }
            else
                getAnimation().setSpriteArray(_idleAnim);
            
            if (Greenfoot.isKeyDown(_keyDown) && isGrounded() && !isGrounded(Ground.class))
                fallOf();
                
            if (Greenfoot.isKeyDown(_keyUp) && isGrounded())
            {
                jump();
                Sound.SelectSound(Sound.PlaySound.JUMP, false);
            }    
                
            if (Greenfoot.isKeyDown(_actionKey))
                _gun.shoot();
        }
    }
    
    /**
     * Overload of move function for the player
     */
    private void move()
    {
        setLocation(getX() + getDirection().getVectorX() * getSpeed(), getY());
        getAnimation().setSpriteArray(_runAnim);
    }
    
    /**
     * Initiates the animation that is passed down on the parameters
     * @param String animName - animation's name
     * @param int nFrames - number of frames in the animation
     */
    private GreenfootImage[] initAnimation(String animName, int nFrames)
    {
        GreenfootImage[] anim = new GreenfootImage[nFrames];
        for (int i = 0; i < nFrames; i++)
        {
            anim[i] = new GreenfootImage(_framesPath + animName + "_" + i + ".png");
        }
        return anim;
    }
    
    /**
     * Checks the player's collision with other objects, be it enemies or anything else
     */
    private void collisionChecker()
    {
        if (getHealth() == 0 && isAlive())
        {
            //Isn't alive anymore
            setIsAlive(false);
            //Removes the gun object from the world
            getWorld().removeObject(this._gun);
            //Initiates the death animation
            getAnimation().setSpriteArray(_deathAnim);
        }
        // get a list of enemies in the world
        List<WalkingEnemy> enemies = getWorld().getObjects(WalkingEnemy.class);
        // check which enimy is the player currently colliding with
        for (WalkingEnemy enemy : enemies) {
            HitBox enemyHitBox = enemy.getHitbox();
            // if it collides with a hitbox takes damage
            if (getHitbox().checkIntersection(enemyHitBox) && !getTookDamage()) {
                // Collision detected   
                takeDamage(1);
                updateHearts();
            }
        }
            
        //Doesn't take damage if is still on invincibility frames
        if (getInvicibilitycounter() == 0 && getTookDamage())
            setTookDamage(false);
            
        if (getTookDamage())
            reduceInvTimer();
    }
    
    /**
     * Displays the players health based on the boolean parameter. If player is alive then it displays player's life, if player is dead displays that it died
     * Used for debug purposes
     * @param boolean alive - true if player alive, false otherwise
     */
    private void displayPlayerStatus(boolean alive)
    {
        if(alive && _secondPlayer)
            getWorld().showText(String.valueOf(getHealth()),10,50);
        else if (alive)
            getWorld().showText(String.valueOf(getHealth()),10,90);
    }
    
    /**
     * Clears the satus that is shown in the screen. "Deletes" it
     * Used for debug purposes
     */
    private void clearPlayerStatus()
    {
        if(_secondPlayer)
            getWorld().showText("",10,50);
        else
            getWorld().showText("",10,90);
    }
    
    /**
     * Updates the player's gun location to the player's actual location
     */
    private void updateGunLocation()
    {
        _gunPos.setVectorX(getX());
        _gunPos.setVectorY(getY()+35);
        _gun.setLocation(_gunPos.getVectorX(),_gunPos.getVectorY());
    }
    
    /**
     * Flips the animation's sprites for whenever the player turns around
     */
    private void flipSpriteArrays()
    {
        // flip Animation sprite arrays
        _idleAnim = getAnimation().flip(_idleAnim);
        _runAnim = getAnimation().flip(_runAnim);
        // flip gun sprite
        this._gun.turnGun();
    }
    
    /**
     * Spawns player's gun and hitbox
     */
    public void prepare()
    {
        spawnGun();
        spawnHitBox();
        spawnHearts();
    }
    
    /**
     * Spawns player's gun in the world
     */
    private void spawnGun()
    {
        getWorld().addObject(_gun,_gunPos.getVectorX(),_gunPos.getVectorY());
    }
    
    /**
     * Checks if player fell off map and will reposition the player at the beginning of the level
     */
    private void fellOffMap()
    {
        if(getY() >= getWorld().getHeight() - 25)
            GameManager.repositionPlayer(this);
    }
    
    /**
     * Keeps creating an object Heart until the loop ends
     */
    public void spawnHearts() 
    {
        int yFirst = 10; // y position of the first player;s hearts
        int ySecond = 40; // y position of the second player's hearts
        int heartOffset = 20; // offset of the hearts in the UI (the spacing between them
        for (int i = 0; i < getHealth(); i++) 
        {
            Heart heart = new Heart();
            getWorld().addObject(heart, 35 + (i * heartOffset), isSecond() ? yFirst : ySecond);
        }
    }

    /**
     * Check which player the heart belongs 
     * and updates the hearts count with the matching health
     */ 
    public void updateHearts() 
    {
        int yFirst = 10; // y position of the first player;s hearts
        int ySecond = 40; // y position of the second player's hearts
        
        List<Heart> hearts = getWorld().getObjects(Heart.class); //gets a list of hearts in the world
        List<Heart> playerHearts = new ArrayList<>();  // list used to put the hearts of each player 
        
        for (Heart heart : hearts) 
        {
            if (heart.getY() == (isSecond() ? yFirst:ySecond))
            {
            playerHearts.add(heart);  // check who belings the heart
            }
        }
        getWorld().removeObjects(playerHearts); // removes all hearts of the player that the heart belongs
        spawnHearts();
    }
    
    public boolean isPlayerTouching(Class <?> c)
    {
        return isTouching(c);
    }
    
    /**
     * Getter for private variable _hasKey
     * @return boolean this._hasKey
     */
    public boolean getHasKey()
    {
        return this._hasKey;
    }
    /**
     * Setter for private variable _hasKey
     */
    public void setHasKey(boolean b)
    {
        this._hasKey = b;
        if(b) GameManager.playersHaveKey();
    }
}
