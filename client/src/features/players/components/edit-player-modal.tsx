import { InputInlineLabel } from '@/components/input-inline-label';
import { callCreatePlayer } from '@/features/players/api/create-player';
import { CreatePlayerResponse, isValidPlayer, Player, PlayerStatus } from '@/types/player';
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
import { ChangeEvent, useEffect, useState } from 'react';
import { useTranslation } from 'react-i18next';
import { useNavigate } from 'react-router-dom';

interface EditPlayerModalProps {
    isOpen: boolean;
    onClose: () => void;
}

export const EditPlayerModal: React.FC<EditPlayerModalProps> = ({
    isOpen,
    onClose,
}) => {
    const { t } = useTranslation();
    const [player, setPlayer] = useState<Player>({
        Status: PlayerStatus.Active,
    } as Player);

    const nav = useNavigate();
    const toast = useToast();
    const [isValid, setIsValid] = useState(false);

    useEffect(() => {
        setIsValid(isValidPlayer(player));
    }, [player]);

    const createPlayer = async () => {
        const json = {
            Status: player.Status,
            Username: player.Username,
        };

        const response = await callCreatePlayer<CreatePlayerResponse>(json);        
        let error = false;
        if (response.success) {
            if (response.data != undefined && response.data?.username != null) {
                toast({
                    title: t('Message.CreateUserSuccess'),
                    status: 'success',
                });
                nav('/player/' + response.data.username);
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

    const handleChange = (event: ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
        const { name, value } = event.target;

        setPlayer((prevPlayer) => ({
            ...prevPlayer,
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
                        <InputInlineLabel label={t('Players.Username')}>
                            <Input
                                name={'Username'}
                                placeholder={t(
                                    'Players.Placeholder.Username'
                                )}
                                onChange={handleChange}
                            />
                        </InputInlineLabel>
                        <InputInlineLabel label={t('Players.Status')}>
                            <Select
                                name={'Status'}
                                placeholder={t('Players.Status')}
                                value={player.Status}
                                onChange={handleChange}
                            >
                                {Object.values(PlayerStatus).map((type) => (
                                    <option key={type} value={type}>
                                        {t('PlayerStatus.' + type)}
                                    </option>
                                ))}
                            </Select>
                        </InputInlineLabel>
                    </VStack>
                </ModalBody>
                <ModalFooter>
                    <Button
                        colorScheme="green"
                        mr={3}
                        onClick={() => {
                            createPlayer();
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
