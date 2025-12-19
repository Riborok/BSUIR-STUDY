library IEEE;
use IEEE.STD_LOGIC_1164.ALL;

-- Uncomment the following library declaration if using
-- arithmetic functions with Signed or Unsigned values
--use IEEE.NUMERIC_STD.ALL;

-- Uncomment the following library declaration if instantiating
-- any Xilinx leaf cells in this code.
--library UNISIM;
--use UNISIM.VComponents.all;

entity lfsr_w_2_dir_test is
--  Port ( );
end lfsr_w_2_dir_test;

architecture Behavioral of lfsr_w_2_dir_test is
    constant N: integer := 5;
    constant Polynom: std_logic_vector(N-1 downto 0) := "00101";
    constant CLK_PERIOD: time := 10 ns;
    
    component lfsr_w_2_dir is
        generic (
            N: integer;
            Polynom: std_logic_vector
        );
        port (
            CLK, RST: in std_logic;
            LOAD, MODE: in std_logic;
            BUS_DATA: inout std_logic_vector(N-1 downto 0)
        );
    end component;
    
    signal CLK: std_logic := '0';
    signal RST: std_logic := '0';
    signal MODE: std_logic := '0';
    signal LOAD: std_logic := '0';
    signal BUS_DATA: std_logic_vector(N-1 downto 0);
    
    signal write_data: std_logic_vector(N-1 downto 0) := (others => 'Z');
begin
    uut: lfsr_w_2_dir
        generic map(
            N => N, 
            Polynom => Polynom
        )
        port map(
            CLK => CLK, 
            RST => RST, 
            MODE => MODE, 
            LOAD => LOAD,
            BUS_DATA => BUS_DATA
        );
        
    BUS_DATA <= write_data when MODE = '1' else (others => 'Z');     
    CLK <= not CLK after CLK_PERIOD/2;

    stim_proc: process
    begin
        RST <= '1';
        wait for CLK_PERIOD*2;
        RST <= '0';
        wait for CLK_PERIOD;
        
        MODE <= '0';
        wait for CLK_PERIOD;
        
        wait for CLK_PERIOD * 10;
        
        MODE <= '1';
        wait for CLK_PERIOD;
        
        write_data <= "01010";
        wait for CLK_PERIOD;
        
        LOAD <= '1';
        wait for CLK_PERIOD;
        LOAD <= '0';
        
        write_data <= (others => 'Z');
        wait for CLK_PERIOD;
        
        MODE <= '0';
        wait for CLK_PERIOD;
        
        wait for CLK_PERIOD * 10;
        
        MODE <= '1';
        wait for CLK_PERIOD;
        
        write_data <= (others => '1');
        wait for CLK_PERIOD;
        
        LOAD <= '1';
        wait for CLK_PERIOD;
        LOAD <= '0';
        
        write_data <= (others => 'Z');
        
        MODE <= '0';
        wait for CLK_PERIOD*2;
        
        report "The end" severity failure;
        wait;
    end process;
end Behavioral;
