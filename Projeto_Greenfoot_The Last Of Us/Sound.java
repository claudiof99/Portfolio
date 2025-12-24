import greenfoot.*;  // (World, Actor, GreenfootImage, Greenfoot and MouseInfo)

/**
 * Sound class is used as a manager for every sound in the game it holds every sound that
 * is used in game into memory to be loaded at runtime
 * 
 * @author Jose Afonso 
 */
public class Sound extends Actor
{
    // Sounds of the game saved into memory
    private static final GreenfootSound _MENU_MUSIC = new GreenfootSound("./sounds/mainMenu.wav"); 
    private static final GreenfootSound _GAME_MUSIC = new GreenfootSound("./sounds/gameMusic.wav");
    private static final GreenfootSound _CLICK_BUTTON = new GreenfootSound("./sounds/clickButton.wav");
    private static final GreenfootSound _GET_HIT = new GreenfootSound("./sounds/hitHurt.wav");
    private static final GreenfootSound _JUMP = new GreenfootSound("./sounds/jump.wav");
    private static final GreenfootSound _SHOOT = new GreenfootSound("./sounds/shoot.wav");
    private static final GreenfootSound _YOU_DIED = new GreenfootSound("./sounds/youDied.mp3");
    private static final GreenfootSound _YOU_WIN = new GreenfootSound("./sounds/youWin.mp3");

    /**
     * Enum type of sounds to be played
     * Used in the selector for which sound to be played
     */
    public static enum PlaySound
    {
        MENU_MUSIC,
        GAME_MUSIC,
        CLICK_BUTTON,
        GET_HIT,
        JUMP,
        SHOOT,
        YOU_DIED,
        YOU_WIN
    }
    /**
     * Selector for the sound to be played or to be stopped with `stopSound`
     * 
     * Uses a switch case to call the correct method for the intended purpose
     * 
     * @param PlaySound sound - Enum from Sound class
     * @param boolean stopSound - Variable that determines if the sound is going to stop or be played
     */
    public static void SelectSound(PlaySound sound, boolean stopSound)
    {
        if(stopSound)
        {
            switch (sound)
            {
                case MENU_MUSIC:
                    musicStop(_MENU_MUSIC);
                    break;
                case GAME_MUSIC:
                    musicStop(_GAME_MUSIC);
                    break;
                case CLICK_BUTTON:
                    musicStop(_CLICK_BUTTON);
                    break;
                case GET_HIT:
                    musicStop(_GET_HIT);
                    break;
                case JUMP:
                    musicStop(_JUMP);
                    break;
                case SHOOT:
                    musicStop(_SHOOT);
                    break;
                case YOU_DIED:
                    musicStop(_YOU_DIED);
                    break;
                case YOU_WIN:
                    musicStop(_YOU_WIN);
                    break;
            }
        }
        else
        {
            switch (sound)
                {
                    case MENU_MUSIC:
                        playSound(_MENU_MUSIC, 30);
                        break;
                    case GAME_MUSIC:
                        playSound(_GAME_MUSIC, 50);
                        break;
                    case CLICK_BUTTON:
                        playSound(_CLICK_BUTTON, 50);
                        break;
                    case GET_HIT:
                        playSound(_GET_HIT, 50);
                        break;
                    case JUMP:
                        playSound(_JUMP, 50);
                        break;
                    case SHOOT:
                        playSound(_SHOOT, 50);
                        break;
                    case YOU_DIED:
                        playSound(_YOU_DIED, 80);
                        break;
                    case YOU_WIN:
                        playSound(_YOU_WIN, 80);
                        break;
                }
        }
    }
    /**
     * Method to play the sound passed as a parameter, can be set a volume as well
     * 
     * @param GreenfootSound sound - sound that will be played, new GreenfootSound("File Path")
     * @param int volume - volume of the sound to be played (0) off (100) loud 
     */
    private static void playSound(GreenfootSound sound, int volume)
    {
        sound.setVolume(volume);
        sound.play();
    }
    /**
     * Method to stop the sound passed as a parameter
     * 
     * @param GreenfootSound sound - sound to be stoped
     */
    public static void musicStop(GreenfootSound sound)
    {
        sound.stop();
    }
}
