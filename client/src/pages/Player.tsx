import { FunctionComponent, ReactElement, useEffect, useState } from 'react'
import { Text } from '@chakra-ui/react'
import { useLoaderData } from 'react-router-dom';
import { convertToUser, User } from '../models/User';

export const Player: FunctionComponent = (): ReactElement => {
	const userData : any = useLoaderData();
	const [user, setUser] = useState<User>({} as User);

	useEffect(() => {
		setUser(convertToUser(userData));		
	}, [userData]);

	return (
		<>			
			<Text>Player {user.Username}.</Text>
		</>
	)
}