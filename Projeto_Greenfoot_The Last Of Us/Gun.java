import greenfoot.*;  // (World, Actor, GreenfootImage, Greenfoot and MouseInfo)

/**
 * Gun class used as an instance of the entity that holds it
 * 
 * @author Joao Oliveira
 */
public class Gun extends Actor
{
    // private int _ammo;
    // private int _reloadTime;
    // weapon specific projectile speed
    private int _projectileSpeed;
    // offset of the barrel
    private int _barrelXOffset;
    private int _barrelYOffset;
    // counter for rate of fire of the gun
    private int _fireRate;
    // fire rate timer of the gun has a setter
    private int _fireRateTime;
    private Vector2 _facingVec;
    // sprite of the gun
    private GreenfootImage _sprite;
    
    /**
     * How the gun acts
     */
    public void act()
    {
        // reduce fire rate counter if the fire rate counter is not already 0
        _fireRate = _fireRate == 0 ? 0 : _fireRate-1;
    }
    
    /**
     * Gun constructor initiates default values for the private variables and a default sprite
     */
    public Gun()
    {
        // this._bullet = new Projectile();
        // this._ammo = 5;
        // this._reloadTime = 5;
        
        _projectileSpeed = 5;
        _barrelXOffset = 25;
        _barrelYOffset = 25;
        _facingVec = Vector2.VectorRIGHT;
        // initiate fire rate variables
        _fireRate = 0;
        _fireRateTime = 40;
        // set image of the gun
        this._sprite = new GreenfootImage("./Weapons/weaponR3.png");
        this._sprite.scale(100,100);
        this.setImage(_sprite);
    }
    
    /**
     * Constructor for gun, adapts the gun's projectile speed and the gun's sprite based on the parameters passed
     * @param int projectileSpeed - speed of the gun's projectiles
     * @param String gunSprite - file path to the gun's sprite
     */
    public Gun(int projectileSpeed, String gunSprite)
    {
        // this._bullet = new Projectile();
        // this._ammo = 5;
        // this._reloadTime = 5;
        
        this._projectileSpeed = projectileSpeed;
        _barrelXOffset = 25;
        _barrelYOffset = 25;
        
        // initiate fire rate variables
        _fireRate = 0;
        
        //TODO ver se este fireRate muda para cada arma
        _fireRateTime = 40;
        // set image of the gun
        this._sprite = new GreenfootImage(gunSprite);
        this._sprite.scale(100,100);
        this.setImage(_sprite);
        
        this._sprite = new GreenfootImage(gunSprite);
    }
    /**
     * method instantiates a projectile
     * 
     * @param direction int value, indicates if the projectile will go left or right
     */
    public void shoot()
    {
        // verify fire rate before shooting again
        if (_fireRate != 0)
            return;
        // start fire rate timer
        startTimer();
        // spawn bullet
        
        Sound.SelectSound(Sound.PlaySound.SHOOT, false);
        getWorld().addObject(new Projectile(
                            _facingVec,_projectileSpeed)
                            , getX() + _barrelXOffset * _facingVec.getVectorX()
                            , getY() + _barrelYOffset
                            );
    }
    
    /**
     * Starts a timer for the fire rate
     */
    private void startTimer()
    {
        _fireRate = _fireRateTime;
    }
    
    /**
     * Flips gun sprite horizontally
     */
    public void turnGun()
    {
        this._sprite.mirrorHorizontally();
    }
    
    /**
     * Setter for the fire rate
     * 
     * @param frate int value of the new fire rate
     * ( fire rate calculated by how long until the next bullet )
     */
    public void setFireRate(int frate)
    {
        _fireRateTime = frate;
    }
    
    /**
     * Sets the gun's facing point for whenever the player turns
     */
    public void setFacingVector(Vector2 vec)
    {
        this._facingVec = vec;
    }
}
