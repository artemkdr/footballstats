import { Select, SelectProps } from '@chakra-ui/react';
import { User } from '../models/User';

interface SelectPlayerProps extends SelectProps {        
    players : User[];    
}

export const SelectPlayer: React.FC<SelectPlayerProps> = (props) => {
    const { players, ...rest } = props;
    return (
        <Select {...rest}>
            {players.map((item, index) => (
                <option key={item.Username} value={item.Username}>{item.Username}</option>
            ))}
        </Select>                            
    )
}