import { CustomLink } from '@/components/custom-link';
import config from '@/config/config';
import { callCreatePlayer } from '@/features/players/api/create-player';
import { EditUserModal } from '@/features/players/modal-player';
import { convertDataToList } from '@/types/list';
import { convertDataToUserList, UserStatus } from '@/types/user';
import {
    Button,
    Heading,
    HStack,
    Text,
    useToast,
    UseToastOptions,
    VStack,
} from '@chakra-ui/react';
import { FunctionComponent, ReactElement, useState } from 'react';
import { useTranslation } from 'react-i18next';
import { useLoaderData } from 'react-router-dom';

export const Players: FunctionComponent = (): ReactElement => {
    const { t } = useTranslation();
    const data = useLoaderData();
    const toast = useToast();
    const usersList = convertDataToUserList(convertDataToList(data)?.List);
    const [isUserModalOpen, setIsUserModalOpen] = useState(false);

    const dict = [
        'leo',
        'rick',
        'charly',
        'nick',
        'bertrand',
        'jules',
        'alex',
        'chris',
        'dima',
        'vova',
        'matt',
        'franck',
        'eva',
        'lena',
        'laura',
        'gwen',
        'alexia',
        'clara',
    ];

    const randomN = (n: number) => {
        return Math.floor(Math.random() * (n + 1));
    };

    const generateUsers = async () => {
        const limit = config.SIMULATE_USERS_NUM;
        let count = 0;
        const loadingToastProps = {
            status: 'loading',
            isClosable: false,
            duration: 30000,
        } as UseToastOptions;
        const loadingToast = toast({
            title: t('Message.GeneratingUsers'),
            description: t('Message.GeneratingCount', { count: count }),
            ...loadingToastProps,
        });
        for (let i = 0; i < limit; i++) {
            const uname =
                dict[randomN(dict.length - 1)] +
                '_' +
                dict[randomN(dict.length - 1)] +
                '_' +
                dict[randomN(dict.length - 1)] +
                '_' +
                randomN(100000);
            const json: any = {
                Username: uname,
                Status: UserStatus.Active,
            };
            callCreatePlayer(json);
            count++;
            if (count % 2 === 0)
                toast.update(loadingToast, {
                    description: t('Message.GeneratingCount', { count: count }),
                    ...loadingToastProps,
                });
        }
        toast.close(loadingToast);
        toast({
            title: t('Message.GeneratingSuccess', { count: count }),
            status: 'success',
        });
        window.location.reload();
    };

    return (
        <VStack spacing={5} align="left">
            <Heading as="h2" size="md">
                {t('Players.Title')}
            </Heading>
            <HStack>
                <Button
                    alignSelf={'start'}
                    colorScheme="green"
                    onClick={() => setIsUserModalOpen(true)}
                >
                    {t('Players.AddNewPlayer')}
                </Button>
                {config.SIMULATE_MODE ? (
                    <Button colorScheme="gray" onClick={() => generateUsers()}>
                        {t('Players.GenerateUsers', {
                            count: config.SIMULATE_USERS_NUM,
                        })}
                    </Button>
                ) : (
                    ''
                )}
            </HStack>
            <VStack spacing={5} align="left" paddingLeft={3}>
                {usersList?.map((item, index) => (
                    <HStack spacing={2} key={index}>
                        <CustomLink
                            link={`/player/${item.Username}`}
                            text={item.Username}
                        />
                        {item.Status !== UserStatus.Active ? (
                            <Text>({t('UserStatus.' + item.Status)})</Text>
                        ) : (
                            ''
                        )}
                    </HStack>
                ))}
            </VStack>
            <EditUserModal
                isOpen={isUserModalOpen}
                onClose={() => setIsUserModalOpen(false)}
            />
        </VStack>
    );
};
