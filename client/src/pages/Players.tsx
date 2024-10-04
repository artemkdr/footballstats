import {
	Button,
	Heading,
	HStack,
	Text,
	VStack
} from '@chakra-ui/react';
import { FunctionComponent, ReactElement, useState } from 'react';
import { useTranslation } from 'react-i18next';
import { useLoaderData } from 'react-router-dom';
import { CustomLink } from '../components/CustomLink';
import { EditUserModal } from '../components/EditUserModal';
import { convertDataToUserList, UserStatus } from '../models/User';


export const Players: FunctionComponent = (): ReactElement => {
	const { t } = useTranslation();	
	const data = useLoaderData();	
	const usersList = convertDataToUserList(data);
	const [ isUserModalOpen, setIsUserModalOpen ] = useState(false);
	
	return (
		<VStack spacing={5} align="left">
			<Heading as="h2" size="md">{t("Players.Title")}</Heading>
			<Button alignSelf={"start"} colorScheme="green" onClick={() => setIsUserModalOpen(true)}>{t('Players.AddNewPlayer')}</Button>				
			<VStack spacing={5} align="left" paddingLeft={3}>
				{usersList?.map((item, index) => (					
					<HStack spacing={2} key={index}>						
						<CustomLink link={`/player/${item.Username}`} text={item.Username} />
						{item.Status !== UserStatus.Active ? 
							<Text>({t("UserStatus." + item.Status)})</Text>
							: ""}
					</HStack>								
				))}
			</VStack>
			<EditUserModal
				isOpen={isUserModalOpen} 				
				onClose={() => setIsUserModalOpen(false)} 
				onCreate={() => {}}				
				/>
		</VStack>
	)
}