import greenfoot.*;  // (World, Actor, GreenfootImage, Greenfoot and MouseInfo)
import java.util.ArrayList;
import java.util.List;
import java.util.HashSet;
import java.util.Set;

/**
 * Write a description of class WalkingEnemy here.
 * 
 * @author Joao Oliveira 
 * @version (a version number or a date)
 */
public class WalkingEnemy extends GravityAffected
{
    // animation
    //private String _currentAnimationFrame;
    private String _framesPath;
    private String _character;
    // different types of animation
    private final int _NFRAMES;
    private GreenfootImage[] _runAnim;
    private GreenfootImage[] _deathAnim;
    private GreenfootImage[] _idleAnim;
    
    private int viewDistance;
    
    /**
     * Animates the enemy
     * Checks the enemy's y axis for ground and platforms
     * Checks enemy's collision with player
     * Constantly updates the enemy's hitbox location
     * Makes the enemy follow the target
     */
    public void act()
    {
        getAnimation().animate(this);
        
        move();
        if(!isGrounded())
            checkYCollision();
        
        checkCollisions();
        
        updateHitBoxLocation();
        
        // setLocation(getX() + getSpeed() * getDirection().getVectorX(), getY());
        // System.out.println(getDirection());
        followTarget();
    }
    
    /**
     * Constructor for enemy: sets the number of animation frames; boolean for if enemy is alive; the enemy's view distance to follow the player; health,
     * direction, speed, etc. Also initializes the enemy's animations
     */
    public WalkingEnemy()
    {
        super();
        _NFRAMES = 6;
        
        setIsAlive(true);
        
        viewDistance = 200;
        
        setHealth(5);
        setDirection(Vector2.VectorLEFT);
        setSpeed(1);
        
        makeHitbox(false);
        
        _character = "Enemy 4/";
        _framesPath = "./Full body animated characters/Enemies/" + _character;
        
        _idleAnim = initAnimation("idle",_NFRAMES);
        _runAnim = initAnimation("walk",_NFRAMES + 1);
        _deathAnim = initAnimation("death",_NFRAMES + 2);
        
        getAnimation().setSpriteArray(_idleAnim);
        
        setSprite(_idleAnim[0]);
        getSprite().scale(200,200);
        
    }
    
    /**
     * Checks the enemy's collisions either with player or with bullets
     */
    private void checkCollisions()
    {
        if(getHealth() == 0 && isAlive())
        {
            setIsAlive(false);
            getAnimation().setSpriteArray(_deathAnim);
            GameManager.addPoints(25);
            return;
        }
        
        //Takes damage if the enemy comes in contact with a bullet
        if(isTouching(Projectile.class) && !getTookDamage())
            takeDamage(1);
        
        if (getInvicibilitycounter() == 0 && getTookDamage())
            setTookDamage(false);
            
        if (getTookDamage())
            reduceInvTimer();
            
        if(getY() >= getWorld().getHeight() - 20)
            takeDamage(getHealth());
    }
    
    /**
     * Move function for the enemy
     */
    private void move() 
    {
        //Leaves function if enemy is dead
        if(!isAlive()) return;
        //Checks if enemy is grounded or at a platform's edge and if so reverses its direction
        if(!isGrounded() || isAtEdge())
        {
            setDirection(Vector2.vectorProduct(getDirection(), Vector2.VectorINVERSE));
            if(!isGrounded()) setLocation(getX(), getY()-2);
        }
        setLocation(getX() + getSpeed() * getDirection().getVectorX(), getY());
    }

    /**
     * Follows the target if the player is within the enemy's field of view
     */
    private void followTarget()
    {
        if(!isAlive()) return;
        if(GameManager.playersAlive() == 0) return;
        Player p = (Player)GameManager.getClosestPlayer(this);
        if(p == null)
            return;
        
        int x = p.getX();
        int y = p.getY();
        
        int distx = (x - getX()) > 0 ? (x - getX()):(x - getX()) * -1;
        
        if(distx < viewDistance && Math.abs(y-getY()) < 20)

        //Will follow the player if the x distance if lesser than the view distance
        if(distx < viewDistance)
        {
            setDirection((x - getX()) > 0 ? Vector2.VectorRIGHT : Vector2.VectorLEFT);
            setLocation(getX() + getSpeed() * getDirection().getVectorX(), getY());
        }
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
            anim[i] = new GreenfootImage(this._framesPath + animName + "_" + i + ".png");
        }
        return anim;
    }
    
    /**
     * Will initialize the enemy's hitbox
     */
    public void prepare()
    {
        spawnHitBox();
    }
}
