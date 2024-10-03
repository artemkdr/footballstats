import { AlertDialog, AlertDialogBody, AlertDialogCloseButton, AlertDialogContent, AlertDialogFooter, AlertDialogHeader, Button } from '@chakra-ui/react'
import { useRef, useState } from 'react'
import { useTranslation } from 'react-i18next'
import { isRouteErrorResponse, useNavigate, useRouteError } from 'react-router-dom'

// If there is no catch-all route, this will be rendered and it will show a 404 error. However in that case, it's not being rendered
// in the <Outlet> of the Root component. So it will fill the whole screen and create an unfriendly user experience.
// For this reason, we have a catch-all route in src\index.tsx with a <NotFound> component that will be rendered in the <Outlet> of the Root component.

type RouterErrorProps = {
	error?: Error; // Make the error prop optional	
};

export const RouterError = ({ error } : RouterErrorProps) => {
	const { t } = useTranslation();
	const nav = useNavigate();
	const cancelRef = useRef(null);	
	const displayedError: unknown = useRouteError() ?? error;
	const [isOpened, setIsOpened] = useState(true);
	const onClose = () => {
		setIsOpened(false); 
		nav("/");
	}
	let errorMessage: string;
		
	if (isRouteErrorResponse(displayedError)) {
		// error is type `ErrorResponse`
		errorMessage = (displayedError as any).error?.message || displayedError.statusText;
	} else if (displayedError instanceof Error) {
		errorMessage = displayedError.message;
	} else if (typeof displayedError === 'string') {
		errorMessage = displayedError;
	} else {		
		errorMessage = t('Message.UnknownError');
	}	
	if ((displayedError as any)?.status === 403 || (displayedError as any)?.cause === 403)
		errorMessage = "ForbidError";
	return (		
		<AlertDialog isOpen={isOpened} onClose={onClose} leastDestructiveRef={cancelRef}>
			<AlertDialogContent>
				<AlertDialogHeader>{t("Message.RouterError")}</AlertDialogHeader>
				<AlertDialogCloseButton />
				<AlertDialogBody>{t("Message." + errorMessage)}</AlertDialogBody>
				<AlertDialogFooter>				
					<Button colorScheme='green' ml={3} onClick={() => nav("/")}>{t("OK")}</Button>					
				</AlertDialogFooter>
			</AlertDialogContent>			
		</AlertDialog>
		
	);
}