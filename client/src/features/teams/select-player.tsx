import { Select, SelectProps } from '@chakra-ui/react';
import { User } from '@/types/user';

interface SelectPlayerProps extends SelectProps {        
    players : User[];    
}

export const SelectPlayer: React.FC<SelectPlayerProps> = (props) => {
    const { players, ...rest } = props;
    return (
        <Select {...rest}>
            {players.map((item) => (
                <option key={item.Username} value={item.Username}>{item.Username}</option>
            ))}
        </Select>                            
    )
}