library IEEE;
use IEEE.STD_LOGIC_1164.ALL;

entity rom_counterr_tb is
end rom_counterr_tb;

architecture Behavioral of rom_counterr_tb is

    constant N     : integer := 4;
    constant DEPTH : integer := 8;

    component rom_counter is
        generic (
            N     : integer := N;
            DEPTH : integer := DEPTH
        );
        port (
            CLK, RST : in  std_logic;
            Q        : out std_logic_vector(N-1 downto 0)
        );
    end component;

    signal CLK : std_logic := '0';
    signal RST : std_logic := '0';
    signal Q   : std_logic_vector(N-1 downto 0);

begin

    uut: rom_counter
        generic map (
            N     => N,
            DEPTH => DEPTH
        )
        port map (
            CLK => CLK,
            RST => RST,
            Q   => Q
        );

    clk_proc : process
    begin
        CLK <= '0'; wait for 5 ns;
        CLK <= '1'; wait for 5 ns;
    end process;

    stimulus : process
    begin
        RST <= '1';
        wait for 15 ns;
        RST <= '0';

        wait for 200 ns;
    end process;

end Behavioral;
