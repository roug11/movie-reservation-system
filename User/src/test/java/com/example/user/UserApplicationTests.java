package com.example.user;

import com.example.user.entity.User;
import com.example.user.repository.UserRepository;
import com.example.user.service.UserService;
import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;
import org.mockito.Mock;
import org.springframework.boot.test.context.SpringBootTest;

import java.util.Optional;

import static org.junit.jupiter.api.Assertions.*;
import static org.mockito.Mockito.*;

@SpringBootTest
class UserApplicationTests {

    @Mock
    private UserRepository userRepository;

    private UserService userService;

    @BeforeEach
    void setUp() {
        userService = new UserService(userRepository);
    }

    @Test
    void contextLoads() {
    }

    @Test
    void testGetUserIban() {
        User user = new User();
        user.setId(1L);
        user.setIban("NL91ABNA0417164300");

        when(userRepository.findById(1L)).thenReturn(Optional.of(user));

        Optional<String> iban = userService.getUserIban(1L);

        assertTrue(iban.isPresent());
        assertEquals("NL91ABNA0417164300", iban.get());
    }

    @Test
    void testGetUserIbanNotFound() {
        when(userRepository.findById(1L)).thenReturn(Optional.empty());

        Optional<String> iban = userService.getUserIban(1L);

        assertFalse(iban.isPresent());
    }

    @Test
    void testAuthenticateSuccess() {
        User user = new User();
        user.setUsername("testuser");
        user.setPassword("password123");

        when(userRepository.findByUsername("testuser")).thenReturn(Optional.of(user));

        Optional<String> token = userService.authenticate("testuser", "password123");

        assertTrue(token.isPresent());
        assertNotNull(token.get());
    }

    @Test
    void testAuthenticateFailure() {
        when(userRepository.findByUsername("testuser")).thenReturn(Optional.empty());

        Optional<String> token = userService.authenticate("testuser", "password123");

        assertFalse(token.isPresent());
    }

    @Test
    void testValidateTokenSuccess() {
        User user = new User();
        user.setUsername("testuser");
        user.setPassword("password123");

        when(userRepository.findByUsername("testuser")).thenReturn(Optional.of(user));

        Optional<String> tokenOptional = userService.authenticate("testuser", "password123");

        assertTrue(tokenOptional.isPresent(), "Token should be present");
        String token = tokenOptional.get();
        boolean isValid = userService.validateToken(token);
        assertTrue(isValid, "Token should be valid");
    }

    @Test
    void testValidateTokenFailure() {
        String invalidToken = "invalid.token.here";

        boolean isValid = userService.validateToken(invalidToken);

        assertFalse(isValid);
    }
}