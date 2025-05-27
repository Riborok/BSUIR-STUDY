import React from 'react';
import { Container, Row, Col, Spinner } from 'react-bootstrap';
import '../css/Loading.css';

const LoadingPage = () => {
    return (
        <Container fluid className="d-flex justify-content-center align-items-center" style={{ height: '100vh' }}>
            <Row className="text-center">
                <Col>
                    <div>
                        <Spinner animation="border" variant="primary" size="lg" />
                    </div>
                    <h3 className="mt-3 loading-text"/>
                </Col>
            </Row>
        </Container>
    );
};

export default LoadingPage;
