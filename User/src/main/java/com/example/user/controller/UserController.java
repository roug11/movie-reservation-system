package com.example.user.controller;

import com.example.user.controller.DTO.TokenRequest;
import com.example.user.controller.DTO.UserPassworRequest;
import com.example.user.service.UserService;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

import java.util.Map;

@RestController
@RequestMapping("/users")
public class UserController {

    private final UserService userService;

    @Autowired
    public UserController(UserService userService) {
        this.userService = userService;
    }

    @GetMapping("/{userId}/iban")
    public ResponseEntity<String> getUserIban(@PathVariable Long userId) {
        return userService.getUserIban(userId)
                .map(ResponseEntity::ok)
                .orElse(ResponseEntity.status(404).body("User not found"));
    }

    @PostMapping("/authenticate")
    public ResponseEntity<String> authenticate(@RequestBody UserPassworRequest credentials) {
        String username = credentials.getUsername();
        String password = credentials.getPassword();
        return userService.authenticate(username, password)
                .map(ResponseEntity::ok)
                .orElse(ResponseEntity.status(401).body("Invalid username or password"));
    }

    @PostMapping("/authorize")
    public ResponseEntity<String> authorize(@RequestBody TokenRequest tokenRequest) {
        String token = tokenRequest.getToken();
        if (userService.validateToken(token)) {
            return ResponseEntity.ok("Authorization successful");
        } else {
            return ResponseEntity.status(HttpStatus.UNAUTHORIZED).body("Invalid token");
        }
    }
}
