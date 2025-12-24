/**
 * Class Vector2 is a class that holds coordinates x and y in a 2D space, it also has the default unit vectors for direction in the x and y axis
 * Also holds methods for calculations that might be needed for the vectors
 * 
 * @author Joao Oliveira, Jose Afonso, Claudio 
 * 
 */
public class Vector2  
{
    // (x,y) of the vector in a 2D space
    private int _x;
    private int _y;

    // Default Vectors
    // vector ONE also bottom right vector
    public static final Vector2 VectorONE = new Vector2(1,1);
    public static final Vector2 VectorLEFT = new Vector2(-1,0);
    public static final Vector2 VectorRIGHT = new Vector2(1,0);
    public static final Vector2 VectorUP = new Vector2(0,-1);
    public static final Vector2 VectorDOWN = new Vector2(0,1);
    public static final Vector2 VectorZERO = new Vector2(0,0);
    // inverse vector also the top left vector
    public static final Vector2 VectorINVERSE = new Vector2(-1,-1);
    /**
     * Constructor for objects of class Vector2
     * Sets vector for deffault (0,0)
     */
    public Vector2()
    {
        this._x = 0;
        this._y = 0;
    }
    /**
     * Constructor overload of Vector 2 objects
     * @param int x - X value of the Vector (coordinates or direction)
     * @param int y - Y value of the Vector (coordinates or direction)
     */
    public Vector2(int x, int y)
    {
        this._x = x;
        this._y = y;
    }
    /**
     * Method to compare Vectors 
     * 
     * vectorObject.isEqual(vectorToCompare)
     * 
     * @param Vector2 v - vector to be compared against
     * @return boolean True if the two vectors are equal (this.x,this.y) == (v.x,v.y)
     */
    public boolean isEqual(Vector2 v)
    {
       return (
                this.getVectorX() == v.getVectorX() && 
                this.getVectorY() == v.getVectorY()
               ); 
    }
    /**
     * Vector product between `vec1` and `vec2`
     * 
     * @param Vector2 vec1
     * @param Vector2 vec2
     * 
     * @return new Vector2 - multiplication between each component of each vector (x,y)
     */
    public static Vector2 vectorProduct(Vector2 vec1, Vector2 vec2)
    {
        return new Vector2(vec1._x * vec2._x ,
                vec1._y * vec2._y);
    }
    /**
     * Overload of toString to print the vector, it is used for debug purposes most of the time
     * 
     * Format: (x,y)
     */
    public String toString()
    {
        return "(" + this._x + "," + this._y + ")";
    }
    /**
     * getter of the vector
     * @return Vector2 - returns a self reference
     */
    public Vector2 getVector()
    {
        return this;
    }
    /**
     * setter of the vector
     * @param Vector2 vec
     */
    public void setVector(Vector2 vec)
    {
        this._x = vec.getVectorX();
        this._y = vec.getVectorY();
    }
    
    public int getVectorX()
    {
        return this._x;
    }
    
    public int getVectorY()
    {
        return this._y;
    }
    
    public void setVectorX(int x)
    {
        this._x = x;
    }
    
    public void setVectorY(int y)
    {
        this._y = y;
    }
}
