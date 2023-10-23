public enum GameState{
    None = 0,
    Menu,
    Play,
    Pause,
    Win,
    Lose
}

public enum TileName{
    None,
    Tile_01,
    Tile_02,
    Tile_03,
    Tile_04,
    Tile_05,
    Tile_06,
    Tile_07,
    Tile_08,
    Tile_09,
    Tile_10,
    Tile_11,
    Tile_12,
}

public enum ScreenDirection
{
    TOP,
    BOTTOM,
    LEFT,
    RIGHT,
    TOP_LEFT,
    TOP_RIGHT,
    BOTTOM_LEFT,
    BOTTOM_RIGHT
}

public enum Sound{

}

public enum SoundId{
    // ui sfx
    s_ui_click = 0,
    s_ui_gift_open,

    // ingame sfx
    s_hover_tile = 10,
    s_select_tile,
    s_explode_tiles,
    s_time_running_out,
    s_combo_multiplier_up,
    s_win,
    s_lose,

    // background music
    // menu music
    m_menu_1 = 50,
    m_menu_2,
    m_menu_3,
    // play music
    m_battle_1 = 60,
    m_battle_2,
    m_battle_3
}

public enum SkillType{
    r_undo,
}