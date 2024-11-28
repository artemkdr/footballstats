import { InputInlineLabel } from '@/components/input-inline-label';
import { callCreatePlayer } from '@/features/players/api/create-player';
import { isValidUser, User, UserStatus } from '@/types/user';
import {
    Button,
    Input,
    Modal,
    ModalBody,
    ModalCloseButton,
    ModalContent,
    ModalFooter,
    ModalHeader,
    ModalOverlay,
    Select,
    useToast,
    VStack,
} from '@chakra-ui/react';
import { useEffect, useState } from 'react';
import { useTranslation } from 'react-i18next';
import { useNavigate } from 'react-router-dom';

interface EditUserModalProps {
    isOpen: boolean;
    onClose: () => void;
}

export const EditUserModal: React.FC<EditUserModalProps> = ({
    isOpen,
    onClose,
}) => {
    const { t } = useTranslation();
    const [user, setUser] = useState<User>({
        Status: UserStatus.Active,
    } as User);

    const nav = useNavigate();
    const toast = useToast();
    const [isValid, setIsValid] = useState(false);

    useEffect(() => {
        setIsValid(isValidUser(user));
    }, [user]);

    const createUser = async () => {
        const json: any = {
            Status: user.Status,
            Username: user.Username,
        };

        const response = await callCreatePlayer(json);
        const responseJson = await response.json();
        let error = false;
        if (response.ok) {
            if (responseJson?.username != null) {
                toast({
                    title: t('Message.CreateUserSuccess'),
                    status: 'success',
                });
                nav('/player/' + responseJson.username);
            } else {
                error = true;
            }
        } else {
            error = true;
        }
        if (error) {
            toast({ title: t('Message.CreateUserError'), status: 'error' });
        }
    };

    const handleChange = (event: any) => {
        const { name, value } = event.target;

        setUser((prevUser) => ({
            ...prevUser,
            [name]: value,
        }));
    };

    return (
        <Modal isOpen={isOpen} onClose={onClose} closeOnOverlayClick={false}>
            <ModalOverlay />
            <ModalContent>
                <ModalHeader>{t('Players.NewPlayer')}</ModalHeader>
                <ModalCloseButton />
                <ModalBody>
                    <VStack spacing={5}>
                        <InputInlineLabel
                            label={t('Players.Username')}
                            input={
                                <Input
                                    name={'Username'}
                                    placeholder={t(
                                        'Players.Placeholder.Username'
                                    )}
                                    onChange={handleChange}
                                />
                            }
                        />
                        <InputInlineLabel
                            label={t('Players.Status')}
                            input={
                                <Select
                                    name={'Status'}
                                    placeholder={t('Players.Status')}
                                    value={user.Status}
                                    onChange={handleChange}
                                >
                                    {Object.values(UserStatus).map((type) => (
                                        <option key={type} value={type}>
                                            {t('UserStatus.' + type)}
                                        </option>
                                    ))}
                                </Select>
                            }
                        />
                    </VStack>
                </ModalBody>
                <ModalFooter>
                    <Button
                        colorScheme="green"
                        mr={3}
                        onClick={() => {
                            createUser();
                        }}
                        isDisabled={!isValid}
                    >
                        {t('Create')}
                    </Button>
                    <Button variant="ghost" onClick={onClose}>
                        {t('Cancel')}
                    </Button>
                </ModalFooter>
            </ModalContent>
        </Modal>
    );
};
