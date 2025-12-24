import greenfoot.*;  // (World, Actor, GreenfootImage, Greenfoot and MouseInfo)

/**
 * Write a description of class Projectile here.
 * 
 * @author Joao Oliveira
 */
public class Projectile extends Actor
{
    // sprite image of the projectile
    private GreenfootImage _sprite;
    // travel speed of the projectile
    private int _travelSpeed;
    // direction to move the projectile in
    private Vector2 _direction;
    
    /**
     * How a bullet moves through the screen and if it is at the edge of the screen
     */
    public void act()
    {
        // move in the direction by the travel speed
        move(this._travelSpeed * this._direction.getVectorX());
        checkEdge();
    }
    /**
     * Prohectile constructor initiates default values for private variables and sets the projectile
     * sprite
     * 
     * @param direction direction to move the projectile in
     * @param tSpeed travel speed of the projectile
     */
    public Projectile(Vector2 direction, int tSpeed)
    {
        this._sprite = new GreenfootImage("./Extras/bullet.png");
        this._sprite.scale(15,15);
        setImage(_sprite);
        
        this._travelSpeed = tSpeed;
        this._direction = direction;
    }
    
    /**
     * Verifies if the projectile hits the edge if true removes its self
     */
    private void checkEdge()
    {
        if(this.isAtEdge())
            getWorld().removeObject(this);
    }
}
