import {
	HStack,
	Heading,
	VStack
} from '@chakra-ui/react';
import { FunctionComponent, ReactElement } from 'react';
import { useTranslation } from 'react-i18next';
import { useLoaderData } from 'react-router-dom';
import { CustomLink } from '../components/CustomLink';
import { convertDataToUserList } from '../models/User';


export const Players: FunctionComponent = (): ReactElement => {
	const { t } = useTranslation();	
	const data = useLoaderData();	
	const usersList = convertDataToUserList(data);
	
	return (
		<VStack spacing={5} align="left">
			<Heading as="h2" size="md">{t("Players.Title")}</Heading>
			<VStack spacing={5} align="left" paddingLeft={3}>
				{usersList?.map((item, index) => (					
					<HStack spacing={2} key={index}>						
						<CustomLink link={`/player/${item.Username}`} text={item.Username} />
					</HStack>								
				))}
			</VStack>
		</VStack>
	)
}