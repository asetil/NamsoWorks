import React, { useState, useEffect, useRef } from 'react';
import { Button, Modal, ModalHeader, ModalBody, ModalFooter } from 'reactstrap';
import RS from '../service/LanguageMapService';

function WModal(props) {
    const [modal, setModal] = useState(props.open || false);
    const toggle = () => setModal(!modal);

    useEffect(() => {
        setModal(props.open);
    }, [props.open]);

    return (
        <Modal isOpen={modal} toggle={toggle} className={props.className} id={props.id}>
            <ModalHeader toggle={toggle}>{RS.get(props.title)}</ModalHeader>
            <ModalBody>
                {props.children}
            </ModalBody>
            {
                props.hasFooter && <ModalFooter>
                    <Button color="primary" onClick={toggle}>Do Something</Button>{' '}
                    <Button color="secondary" onClick={toggle}>Cancel</Button>
                </ModalFooter>
            }
        </Modal>
    );
}

export default WModal;