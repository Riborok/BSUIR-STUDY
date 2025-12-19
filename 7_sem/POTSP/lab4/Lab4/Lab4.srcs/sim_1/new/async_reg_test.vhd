library IEEE;
use IEEE.STD_LOGIC_1164.ALL;

-- Uncomment the following library declaration if using
-- arithmetic functions with Signed or Unsigned values
--use IEEE.NUMERIC_STD.ALL;

-- Uncomment the following library declaration if instantiating
-- any Xilinx leaf cells in this code.
--library UNISIM;
--use UNISIM.VComponents.all;

entity async_reg_test is
--  Port ( );
end async_reg_test;

architecture Behavioral of async_reg_test is
    constant N_BITS: integer := 8;
    
    component async_reg_beh is
        generic (
            N: integer := N_BITS
        );
        port (
            EN: in std_logic;
            Din: in std_logic_vector(N_BITS-1 downto 0);
            Dout: out std_logic_vector(N_BITS-1 downto 0)
        );
     end component;
    
     component async_reg_struct is
         generic (
             N: integer := N_BITS
         );
         port (
             EN: in std_logic;
             Din: in std_logic_vector(N_BITS-1 downto 0);
             Dout: out std_logic_vector(N_BITS-1 downto 0)
         );
     end component;
     
     signal EN: std_logic := '0';
     signal Din: std_logic_vector(N_BITS-1 downto 0) := (others => '0');
     
     signal Dout_beh: std_logic_vector(N_BITS-1 downto 0);
     signal Dout_struct: std_logic_vector(N_BITS-1 downto 0);
     
     signal Error: std_logic;
begin
    uut_beh: async_reg_beh
        port map (
            EN => EN,
            Din => Din,
            Dout => Dout_beh
        );

    uut_struct: async_reg_struct
        port map (
            EN => EN,
            Din => Din,
            Dout => Dout_struct
        );

    Error <= '1' when (Dout_beh /= Dout_struct) else '0';
    
    stim_proc : process
    begin        
        EN <= '0';
        Din <= x"AA";
        wait for 20 ns;

        EN <= '1';
        Din <= x"55";
        wait for 20 ns;
        Din <= x"F0";
        wait for 20 ns;

        EN <= '0';
        wait for 10 ns;
        Din <= x"12";
        wait for 20 ns;
        
        if Error = '1' then
            report "Behaviour and structural are not equal" severity failure;
        else
            report "Òhe end" severity failure;
        end if;
        
        wait;
    end process;
    
end Behavioral;
