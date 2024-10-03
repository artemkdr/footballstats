import { FunctionComponent, ReactElement, useEffect, useState } from 'react'
import { Heading, Text, VStack } from '@chakra-ui/react'
import { useLoaderData } from 'react-router-dom';
import { convertToUser, User } from '../models/User';
import { useTranslation } from 'react-i18next';

export const Player: FunctionComponent = (): ReactElement => {
	const userData : any = useLoaderData();
	const [user, setUser] = useState<User>({} as User);
	const { t } = useTranslation();	

	useEffect(() => {
		setUser(convertToUser(userData));		
	}, [userData]);

	return (
		<VStack spacing={5} align="left">
			<Heading as="h2" size="md">{t("Player")} {user.Username}</Heading>			
		</VStack>
	)
}