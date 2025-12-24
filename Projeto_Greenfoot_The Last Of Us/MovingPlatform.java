import greenfoot.*;  // (World, Actor, GreenfootImage, Greenfoot and MouseInfo)
import java.util.List;
import java.util.Set;
import java.util.ArrayList;
import java.util.HashSet;

/**
 * Write a description of class PlatformHorizontal here.
 * 
 * @author (your name) 
 * @version (a version number or a date)
 */
public class MovingPlatform extends Platform
{
    
    private int _lowerRange;
    private int _upperRange;
    
    private Vector2 _platformVec;
    /**
     * Makes the platform move within the lower and upper range
     */
    public void act()
    {
        move(_lowerRange, _upperRange);
    }
    
    /**
     * Constructor for the MovingPlatform class. The platform will move horizontally or vertically based on the vector passed as a parameter
     * @param Vector2 vec2 - Vector that determines the platform's direction
     */
    public MovingPlatform(Vector2 vec2)
    {
        this._lowerRange = 0;
        this._upperRange = 0;
        
        setSpeed(2);
        setDirection(1);
        
        if (vec2.isEqual(Vector2.VectorRIGHT) || vec2.isEqual(Vector2.VectorLEFT))
        {
            _platformVec = vec2;
        }
        else if (vec2.isEqual(Vector2.VectorUP) || vec2.isEqual(Vector2.VectorDOWN))
        {
            _platformVec = vec2;
        }
    }
    
    /**
     * Overloading the constructor with the lower and upper limits to determine the platform's moving coordinates
     * @param Vector2 vec2 - Vector that determines the platform's direction
     * @param int lowerLimit - lower range for the platform
     * @param int upperLimit - upper range for the platform
     */
    public MovingPlatform(Vector2 vec2, int lowerLimit, int upperLimit)
    {
        this(vec2);
        this._lowerRange = lowerLimit;
        this._upperRange = upperLimit;
    }
    
    /**
     * Default constructor for the MovingPlatform class
     */
    public MovingPlatform()
    {
        
        this._lowerRange = 0;
        this._upperRange = 0;
        
        setSpeed(2);
        setDirection(1);
        _platformVec = _platformVec == null ? Vector2.VectorZERO:_platformVec;
    }
    
    /**
     * Move function for the platform's movement based on a lower and upper range
     * @param lowerRange - lower range of movement
     * @param upperRange - upper range of movement
     */
    private void move(int lowerRange, int upperRange)
    {
        if (
            ( (lowerRange != 0 && upperRange != 0) && withinRange()) || 
            isAtEdge() || isTouching(Platform.class)
            )
            invertDirection();
        setLocation(getX() + _platformVec.getVectorX() * getSpeed(),
                    getY() + _platformVec.getVectorY() * getSpeed());
        movePlayersOnTop();
    }
    
    /**
     * Function that allows the player to move on top of the platform
     */
    private void movePlayersOnTop()
    {
        // ajust a pixel before checking if there is anything above
        setLocation(getX(), getY()-1);
        // get the hitbox because the Player image(sprite) is oversized
        // check multiple positions to ensure detection
        List<HitBox> hitBoxes = new ArrayList<>();
        hitBoxes.addAll(getObjectsAtOffset(0, getImage().getHeight() / 2 - 50, HitBox.class));  // Center
        hitBoxes.addAll(getObjectsAtOffset(-30, getImage().getHeight() / 2 - 50, HitBox.class)); // Left
        hitBoxes.addAll(getObjectsAtOffset(30, getImage().getHeight() / 2 - 50, HitBox.class));  // Right

        // remove duplicates (since the same hitbox might be detected multiple times)
        Set<HitBox> uniqueHitBoxes = new HashSet<>(hitBoxes);

        // iterate over all the hitboxes found at the given offset
        for (HitBox playerHitBox : uniqueHitBoxes) {
            if (playerHitBox != null && playerHitBox.getOwner() != null && playerHitBox.getOwner() instanceof Player) {
                // get the owner of the hitbox and update their location
                playerHitBox.getOwner().setLocation(
                    playerHitBox.getOwner().getX() + _platformVec.getVectorX() * getSpeed(), // set location based on current location and direction * speed to keep up
                    playerHitBox.getOwner().getY() + _platformVec.getVectorY() * getSpeed()  // since we are using vectors we simply get the X and Y respectivaly to what we need
                );
            }
        }
        setLocation(getX(), getY()+1);
    }
    
    /**
     * Inverts the platform's direction
     */
    private void invertDirection()
    {
        _platformVec = Vector2.vectorProduct(_platformVec, Vector2.VectorINVERSE);
    }
    
    /**
     * Boolean that checks if the platform is within the lower and upper range determined
     */
    private boolean withinRange()
    {
        return (getY() == _lowerRange || getX() == _lowerRange) || 
            (getY() == _upperRange || getX() == _upperRange);
    }
    
    /**
     * Sets the platform's lower and upper range
     * @param int lower - lower range of movement
     * @param int upper - upper range of movement
     */
    public void setRange(int lower, int upper)
    {
        this._lowerRange = lower;
        this._upperRange = upper;
    }
}
